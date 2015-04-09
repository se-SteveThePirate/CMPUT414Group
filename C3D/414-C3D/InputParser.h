#pragma once
#include <string>
#include <vector>
#include "Frame.h"

class InputParser
{
public:
	std::vector<std::string> labels;
	std::vector<float> points;
	int numFrames;

	InputParser(char *fileName);
	void _split_labels(std::string line, std::vector<std::string>* split);

};

