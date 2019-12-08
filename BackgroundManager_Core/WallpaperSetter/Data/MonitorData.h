#pragma once
#include <vector>

struct Monitor {
    int left;
    int top;
    int right;
    int bottom;
};


struct MonitorInfo {
    int count;
    std::vector<Monitor> monitors;
};