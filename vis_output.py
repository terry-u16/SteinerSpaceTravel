import argparse
import matplotlib.pyplot as plt

parser = argparse.ArgumentParser()
parser.add_argument("-s", "--seed", type=int, required=True)
parser.add_argument("-o", "--output", type=str, required=True)
args = parser.parse_args()

seed = args.seed
output = args.output
points = []

with open(f"tools/in/{seed:04}.txt") as f:
    n, m = map(int, f.readline().split())
    for i in range(n):
        x, y = map(int, f.readline().split())
        marker = "*" if i == 0 else "."
        marker_size = 15 if i == 0 else 8
        plt.plot(x, y, marker, markersize=marker_size)
        points.append((x, y))

with open(output) as f:
    for _ in range(m):
        x, y = map(int, f.readline().split())
        plt.plot(x, y, "+", markersize=12)
        points.append((x, y))
    k = int(f.readline())
    orders = []
    for _ in range(k):
        orders.append(int(f.readline()))

    for i in range(k - 1):
        prev = orders[i]
        next = orders[i + 1]
        x0, y0 = points[prev]
        x1, y1 = points[next]
        plt.plot([x0, x1], [y0, y1], color="black")

    plt.xlim((0, 1000))
    plt.ylim((0, 1000))
    plt.show()
