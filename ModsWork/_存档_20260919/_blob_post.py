# -*- coding: utf-8 -*-
"""直接用 urllib（绕开代理）向 GitHub 提交单个 blob，用于绕过 gh 对大请求体的限制。"""
import base64, json, os, subprocess, sys, urllib.request

REPO = "Moyxi/KDQSHH"
tok = subprocess.check_output(["gh", "auth", "token"]).decode().strip()
op = urllib.request.build_opener(urllib.request.ProxyHandler({}))

local = sys.argv[1]
payload = base64.b64encode(open(local, "rb").read()).decode("ascii")
print("负载 base64 长度：%d" % len(payload))

req = urllib.request.Request(
    "https://api.github.com/repos/%s/git/blobs" % REPO,
    data=json.dumps({"encoding": "base64", "content": payload}).encode("ascii"),
    headers={"Authorization": "Bearer " + tok,
             "Accept": "application/vnd.github+json",
             "Content-Type": "application/json",
             "User-Agent": "kd"},
    method="POST")

try:
    with op.open(req, timeout=1800) as r:
        res = json.load(r)
    print("sha=%s" % res["sha"])
except urllib.error.HTTPError as e:
    body = e.read().decode("utf-8", "replace")[:500]
    print("HTTP %s: %s" % (e.code, body))
