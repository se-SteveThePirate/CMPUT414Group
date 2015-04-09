#include "Point.h"
#include <vector>


class Frame
{
public:
	std::vector<Point> points;
	Frame(void);
	void add_point(double x, double y, double z);
};

