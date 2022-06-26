#include <set>
#include <tuple>
#include <cassert>
#include "testlib.h"

int main(int argc, char** argv) {
    registerValidation(argc, argv);

    int n = inf.readInt(100, 100);
    int m = inf.readInt(8, 8);
    inf.readEoln();
    std::set<std::pair<int, int>> set;

    for (int i = 0; i < n; i++)
    {
        int a = inf.readInt(0, 1000);
        int b = inf.readInt(0, 1000);
        inf.readEoln();

        set.insert(std::make_pair(a, b));
    }

    assert(set.size() == n);

    inf.readEof();
    return 0;    
}