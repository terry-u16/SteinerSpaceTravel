import argparse
import matplotlib.pyplot as plt

parser = argparse.ArgumentParser()
parser.add_argument("-s", "--seed", type=int, required=True)
args = parser.parse_args()

seed = args.seed
with open(f"tools/in/{seed:04}.txt") as f:
    n, _ = map(int, f.readline().split())
    for i in range(n):
        x, y = map(int, f.readline().split())
        marker = "*" if i == 0 else "."
        marker_size = 15 if i == 0 else 8
        plt.plot(x, y, marker, markersize=marker_size)
    plt.xlim((0, 1000))
    plt.ylim((0, 1000))
    plt.show()
