namespace CustomKnight
{
    internal class SpriteLoader
    {
        private static bool texRoutineRunning;
        private static Coroutine setTexRoutine;
        // 当前正在执行的流式加载协程及其版本号：换肤时会停止旧协程，防止新旧贴图交错写入
        private static Coroutine loadTexRoutine;
        private static int loadVersion;
        internal static bool LoadComplete { get; private set; }
        internal static void PullDefaultTextures()
        {
            if (!SkinManager.savedDefaultTextures)
            {
                foreach (KeyValuePair<string, Skinable> kvp in SkinManager.Skinables)
                {
                    kvp.Value?.SaveTexture();
                }
            }
            SkinManager.savedDefaultTextures = true;
        }
        internal static void UnloadAll()
        {
            if (HeroController.instance != null)
            {
                foreach (KeyValuePair<string, Skinable> kvp in SkinManager.Skinables)
                {
                    kvp.Value.Reset();
                }
            }

            // 若流式加载仍在进行，先停止该协程并作废旧加载，避免卸载后残留
            if (loadTexRoutine != null && GameManager.instance != null)
            {
                CoroutineHelper.GetRunner().StopCoroutine(loadTexRoutine);
                loadTexRoutine = null;
                loadVersion++;
            }

            if (texRoutineRunning && GameManager.instance != null)
            {
                CoroutineHelper.GetRunner().StopCoroutine(setTexRoutine);
                texRoutineRunning = false;
            }

            DestroyObjects();
        }
        internal static void Load()
        {
            // 平滑加载模式下：每次换肤自增版本号，并停止上一轮仍在执行的流式加载协程，避免交错
            if (CustomKnight.GlobalSettings.SmoothTextureLoading)
            {
                if (loadTexRoutine != null)
                {
                    CoroutineHelper.GetRunner().StopCoroutine(loadTexRoutine);
                    loadTexRoutine = null;
                }
                loadVersion++;
                loadTexRoutine = CoroutineHelper.GetRunner().StartCoroutine(Start(loadVersion));
            }
            else
            {
                // 关闭平滑加载时：先停止并作废仍在进行的平滑加载，避免与同步加载交错写入贴图
                if (loadTexRoutine != null)
                {
                    CoroutineHelper.GetRunner().StopCoroutine(loadTexRoutine);
                    loadTexRoutine = null;
                    loadVersion++;
                }
                // 完全保持原版同步加载流程
                CoroutineHelper.GetRunner().StartCoroutine(Start());
            }
        }
        internal static void ModifyHeroTextures(SaveGameData data = null)
        {
            if (!texRoutineRunning)
            {
                setTexRoutine = CoroutineHelper.GetRunner().StartCoroutine(SetHeroTex());
                texRoutineRunning = true;
            }
        }

        internal static IEnumerator Start(int version = 0)
        {
            // 平滑模式（version 非 0）与原版一样，在加载前先执行一次全局 Swap，保证场景物件换肤完整
            bool smoothLoading = version != 0;
            CustomKnight.swapManager.SkinChangeSwap(SkinManager.CurrentSkin);
            yield return new WaitWhile(
                () => HeroController.instance == null || GameManager.instance == null || GameManager.instance.gameMap == null
            );
            if (smoothLoading)
            {
                // 等待期间若已被更新的换肤取代，则直接放弃本次加载
                if (version != loadVersion)
                {
                    yield break;
                }
                yield return LoadSpritesSmooth(version);
                if (version == loadVersion)
                {
                    loadTexRoutine = null;
                }
            }
            else
            {
                LoadSprites();
            }
        }

        private static void DestroyObjects()
        {
            foreach (KeyValuePair<string, Skinable> pair in SkinManager.Skinables)
            {
                CustomKnightTexture texture = pair.Value.ckTex;
                if (texture.tex != null)
                {
                    GameObject.Destroy(texture.tex);
                }
            }
            LoadComplete = false;
        }

        internal static void SetSkin(Dictionary<string, Skinable> SkinableMap)
        {
            SkinManager.Skinables = SkinableMap;
            ModifyHeroTextures();
        }
        internal static void LoadSprites()
        {
            LoadComplete = false;
            if (SkinManager.CurrentSkin == null)
            {
                SkinManager.CurrentSkin = SkinManager.GetDefaultSkin();
            }
            if (SkinManager.CurrentSkin.shouldCache())
            {
                TextureCache.recentSkins.Add(SkinManager.CurrentSkin.GetId());
            }
            foreach (KeyValuePair<string, Skinable> kvp in SkinManager.Skinables)
            {
                kvp.Value.Prepare();
                CustomKnightTexture texture = kvp.Value.ckTex;
                if (TextureCache.skinTextureCache.TryGetValue(SkinManager.CurrentSkin.GetId(), out var skinCache) && skinCache.TryGetValue(texture.fileName, out var cachedTex))
                {
                    texture.tex = cachedTex.tex;
                    texture.missing = cachedTex.missing;
                    continue;
                }
                texture.missing = !SkinManager.CurrentSkin.Exists(texture.fileName);
                if (!texture.missing)
                {
                    texture.tex = SkinManager.CurrentSkin.GetTexture(texture.fileName);
                    if (SkinManager.CurrentSkin.shouldCache())
                    {
                        TextureCache.setSkinTextureCache(SkinManager.CurrentSkin.GetId(), texture.fileName, new CustomKnightTexture(texture.fileName, texture.missing, texture.defaultTex, texture.tex));
                    }
                }
                else
                {
                    // 当前皮肤缺少此贴图：先尝试自动补齐（从其它皮肤随机挑同名贴图）
                    if (!AutoFill.TryFill(texture, SkinManager.CurrentSkin))
                    {
                        texture.tex = null;
                    }
                }
            }
            TextureCache.trimTextureCache();
            if (!SkinManager.Skinables[Hud.NAME].ckTex.missing && !SkinManager.Skinables[OrbFull.NAME].ckTex.missing)
            {
                SaveHud.GenerateSaveHud(SkinManager.Skinables[Hud.NAME].ckTex.currentTexture, SkinManager.Skinables[OrbFull.NAME].ckTex.currentTexture);
            }
            SetSkin(SkinManager.Skinables);
            LoadComplete = true;
        }

        /// 平滑模式加载贴图：将原 LoadSprites 的全量同步解码拆分为逐帧执行（按时间与数量预算），
        /// 避免一次性读取解码上百张贴图造成换肤卡顿。
        /// 加载期间不施加任何贴图，全部完成后才统一 SetSkin/Apply，皮肤一次到位。
        private static IEnumerator LoadSpritesSmooth(int version)
        {
            LoadComplete = false;
            if (SkinManager.CurrentSkin == null)
            {
                SkinManager.CurrentSkin = SkinManager.GetDefaultSkin();
            }
            if (SkinManager.CurrentSkin.shouldCache())
            {
                TextureCache.recentSkins.Add(SkinManager.CurrentSkin.GetId());
            }

            // 帧预算：超过指定毫秒数或解码张数就让出本帧，保证游戏运行流畅
            var budgetMs = Mathf.Max(1, CustomKnight.GlobalSettings.SmoothTextureBudgetMs);
            var batchSize = Mathf.Max(1, CustomKnight.GlobalSettings.SmoothTextureBatchSize);
            var frameStartTime = Time.realtimeSinceStartup;
            var decodedThisFrame = 0;

            foreach (KeyValuePair<string, Skinable> kvp in SkinManager.Skinables)
            {
                // 中途换肤：若版本号已变化，立即作废本次加载，交给新的加载流程处理
                if (version != loadVersion)
                {
                    yield break;
                }

                kvp.Value.Prepare();
                CustomKnightTexture texture = kvp.Value.ckTex;
                if (TextureCache.skinTextureCache.TryGetValue(SkinManager.CurrentSkin.GetId(), out var skinCache) && skinCache.TryGetValue(texture.fileName, out var cachedTex))
                {
                    // 缓存命中：直接复用已有贴图，无磁盘读取与解码开销
                    texture.tex = cachedTex.tex;
                    texture.missing = cachedTex.missing;
                    continue;
                }
                texture.missing = !SkinManager.CurrentSkin.Exists(texture.fileName);
                if (!texture.missing)
                {
                    // 磁盘读取 + 贴图解码（慢操作），只处理少量后就让出一帧
                    texture.tex = SkinManager.CurrentSkin.GetTexture(texture.fileName);
                    // 优先加载 Knight.png：解码后立即 Apply 到主角，不等其余贴图全部就绪
                    if (kvp.Key == Knight.NAME)
                    {
                        kvp.Value.Apply();
                    }
                    if (SkinManager.CurrentSkin.shouldCache())
                    {
                        TextureCache.setSkinTextureCache(SkinManager.CurrentSkin.GetId(), texture.fileName, new CustomKnightTexture(texture.fileName, texture.missing, texture.defaultTex, texture.tex));
                    }
                    decodedThisFrame++;
                    if (decodedThisFrame >= batchSize || (Time.realtimeSinceStartup - frameStartTime) * 1000f >= budgetMs)
                    {
                        yield return null;
                        frameStartTime = Time.realtimeSinceStartup;
                        decodedThisFrame = 0;
                    }
                }
                else
                {
                    // 当前皮肤缺少此贴图：先尝试自动补齐（从其它皮肤随机挑同名贴图）
                    if (AutoFill.TryFill(texture, SkinManager.CurrentSkin))
                    {
                        // 补齐同样属于贴图解码，计入本帧预算，避免长时间占用主线程
                        decodedThisFrame++;
                        if (decodedThisFrame >= batchSize || (Time.realtimeSinceStartup - frameStartTime) * 1000f >= budgetMs)
                        {
                            yield return null;
                            frameStartTime = Time.realtimeSinceStartup;
                            decodedThisFrame = 0;
                        }
                    }
                    else
                    {
                        texture.tex = null;
                    }
                }
            }

            // 收尾前再次确认没有被新的换肤取代
            if (version != loadVersion)
            {
                yield break;
            }
            TextureCache.trimTextureCache();
            // 平滑模式不在换肤瞬间同步生成 SaveHud（大图翻转移位耗时长），
            // 保留原触发时机之外的行为：存档选择界面（PresentSaveSlot）会按需生成，功能不变且不卡顿。
            SetSkin(SkinManager.Skinables);
            LoadComplete = true;
        }

        private static IEnumerator SetHeroTex()
        {
            yield return new WaitWhile(() => DefaultSkin.isGeneratingDefaultSkin || !LoadComplete || HeroController.instance == null || CharmIconList.Instance == null);
            /*foreach(var psr in GameObject.FindObjectsOfType<ParticleSystemRenderer>())
            {
                psr.gameObject.LogWithChildren();
                DumpManager.debugDumpTex((Texture2D)psr.material.mainTexture,psr.name);
            }*/
            PullDefaultTextures();
            CustomKnight.swapManager.SkinChangeSwap(SkinManager.CurrentSkin);
            foreach (KeyValuePair<string, Skinable> kvp in SkinManager.Skinables)
            {
                kvp.Value.Apply();
            }
            texRoutineRunning = false;
        }



    }
}
