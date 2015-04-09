#include "Frame.h"
#include "Point.h"
#include <vector>


Frame::Frame()
{
	points = std::vector<Point>();
}

void Frame::add_point(double x, double y, double z){
	points.push_back(Point(x, y, z));
}
