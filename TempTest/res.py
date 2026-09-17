import re, subprocess, sys
# Use strings + grep to find manifest resource names in DLL
for dll in sys.argv[1:]:
    res = subprocess.run(["strings", dll], capture_output=True, text=True, errors="ignore")
    # find patterns that look like manifest resource names
    names = set()
    for m in re.finditer(r'FiveKnights\.StreamingAssets\.', res.stdout):
        # get surrounding context
        ctx = res.stdout[m.start():m.start()+80]
        names.add(ctx.split('\n')[0].split('\0')[0].split('"')[0])
    print(f"=== {dll} ===")
    if names:
        for n in sorted(names):
            print(" ", n)
    else:
        print("  (no FiveKnights.StreamingAssets resources found)")
