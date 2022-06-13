import argparse
import matplotlib.pyplot as plt

parser = argparse.ArgumentParser()
parser.add_argument("-s", "--seed", type=int, required=True)
args = parser.parse_args()

seed = args.seed
with open(f"tools/in/{seed:04}.txt") as f:
    n, _ = map(int, f.readline().split())
    plt.plot(500, 500, "*", markersize=15)
    for _ in range(n):
        x, y = map(int, f.readline().split())
        plt.plot(x, y, ".")
    plt.xlim((0, 1000))
    plt.ylim((0, 1000))
    plt.show()
