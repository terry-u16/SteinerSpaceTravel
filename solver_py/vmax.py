for _ in range(8):
    print("0 0")

V = 100000
print(V)

for i in range(V - 1):
    print(f"1 {i % 100 + 1}")

print(f"1 1")