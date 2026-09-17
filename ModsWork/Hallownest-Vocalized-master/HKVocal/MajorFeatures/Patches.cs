using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKVocals.MajorFeatures
{
    public class Patches
    {
        internal static void Hook()
        {
            ModHooks.LanguageGetHook += LanguageGetPatches;
        }

        private static bool IsChinese => Language.Language.CurrentLanguage().ToString() == "ZH";

        private static string LanguageGetPatches(string key, string sheetTitle, string orig)
        {
            if (key == "SHOP_DESC_NOTCH_4")
                return orig.Replace("more charms", "more of your charms");
            if (key == "HORNET_FOUNTAIN_1")
                return IsChinese
                    ? "又见面了，小幽灵。<page>我向来洞察敏锐，却曾低估了你；不过如今我已猜到真相。<page>你已见识过这个王国之外的世界。你的坚韧源于两重虚空。<page>难怪你能抵达这个世界的心脏。如此一来，你也将知晓维系它所需的牺牲。"
                    : "Again we meet little ghost.<page>I'm normally quite perceptive. You I underestimated, though I've since guessed the truth.<page>You've seen beyond this kingdom's bounds. Yours is resilience born of two voids.<page>It's no surprise then you've managed to reach the heart of this world. In doing so, you shall know the sacrifice that keeps it standing.";
            if (key == "WITCH_REWARD_2A")
                return IsChinese
                    ? "啊……你的梦之钉已积蓄超过 200 点精华，证明了你收集精华的天赋。<page>你看见外面那扇巨门了吗？我的族人很久以前封上了它，并禁止任何人开启。<page>不过据我所知，我似乎是族中唯一还活着的人。那我便不必为打破禁忌而内疚。为了证明我对你的信任，我会打开那扇门。"
                    : "Ahhh... Your Dream Nail holds over 200 Essence. You're proving your talent in its collection.<page>Have you seen that great door just outside? My tribe closed it long ago and forbade its opening.<page>Ah, but as far as I can tell I'm the only member of my tribe still breathing. That means I mustn't feel bad about breaking a taboo. As proof of my belief in you, I'll open the door.";
            if (key == "WATERWAYS_GREET")
                return IsChinese
                    ? "呵呵！这些水道多么令人兴奋啊，简直是由管道与隧道构成的迷宫。<page>我再也找不到比这里更适合施展本领的地方了。一切都如此规整周详，与那些洞窟粗陋无序的样子完全不同。<page>啊，不过真可惜，我的鼻子告诉我真菌荒地近在咫尺；我想这趟潮湿的冒险快要结束了。看来该为这次旅程画上句号啦。"
                    : "Ho ho! Aren't these waterways thrilling? A labyrinth of pipes and tunnels.<page>I couldn't have asked for a better place to employ my talents. It's all so orderly, so considered, nothing like the crude irregularity of those caverns.<page>Ahh but so sad, my trunk is telling me those Fungal Wastes are close and I sense my damp adventure may have reached its end. Guess I'll be calling this one done.";
            if (key == "WATERWAYS_BOUGHT")
                return IsChinese
                    ? "我敢说这些管道与房间过去是用来运送城市废物的。那时这里肯定臭气熏天。幸好连绵不断的雨水已经将它们冲洗干净。"
                    : "I'd wager these pipes and chambers were once used to carry the city's waste. Would've been a horrid stench down here. Thankfully that constant rain has flushed them clean. ";
            return orig;
        }
    }
}
