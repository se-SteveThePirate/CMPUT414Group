// 414-C3D.cpp : Defines the entry point for the console application.
//


#include "stdafx.h"
#include "Objbase.h" 
#include "InputParser.h"
#include <stdio.h>
#include <iostream> 
#include <sstream>

#import "c3dserver.dll"

using namespace std;
void CreateSafeArray(SAFEARRAY** s, float * data, int len);


int _tmain(int argc, _TCHAR* argv[])
{
	CoInitialize(NULL);
	{
	C3DSERVERLib::IC3DPtr p(_uuidof(C3DSERVERLib::C3D));




	InputParser ip = InputParser("testRelativeToHeadJoint.txt");
	float *x, *y, *z, **channelsx, **channelsy, **channelsz;
	int count;


#ifdef _DEBUGG
	std::cout << "Labels:\n\n";
	for (std::vector<std::string>::iterator it = ip.labels.begin(); it != ip.labels.end(); ++it) {
		std::cout << *it << '\n';
		Sleep(10);
	}

	std::cout << "Points:\n\n";
	for (std::vector<float>::iterator it = ip.points.begin(); it != ip.points.end(); it = it + 3) {
		std::cout << *it << " " << *(it + 1) << " " << *(it + 2) << "\n";
		Sleep(10);
	}
#endif

	const wchar_t* sFile = L"C:\\Users\\Owner\\Desktop\\414\\414-C3D\\Debug\\output.c3d";
	_bstr_t bStrFile = _bstr_t(sFile);


	p->NewFile(bStrFile, // File name
					  1, // File type = intel
					  2, // Data type is floating point
					  0, // 0 analog channels
					  0, // 0 analog frames per point frame
					  ip.labels.size(), // As many markers as labels
					  30, // 30 FPS
					  1, // Scaleing of points is 1
					  ip.numFrames); // Total number of frames

	count = ip.numFrames * ip.labels.size();

	x = new float[count];
	y = new float[count];
	z = new float[count];

	channelsx = new float*[ip.labels.size()];
	channelsy = new float*[ip.labels.size()];
	channelsz = new float*[ip.labels.size()];

	for (int i = 0; i < ip.labels.size(); i++)
	{
		channelsx[i] = new float[ip.numFrames];
		channelsy[i] = new float[ip.numFrames];
		channelsz[i] = new float[ip.numFrames];
	}

	for (int i = 0; i < count; i++) 
	{
		x[i] = ip.points[3 * i];
		y[i] = ip.points[3 * i + 1];
		z[i] = ip.points[3 * i + 2];

	}

	for (int i = 0; i < ip.labels.size(); i++){
		for (int j = 0; j < ip.numFrames; j++){
			channelsx[i][j] = x[j*ip.labels.size()+i];
			channelsy[i][j] = y[j*ip.labels.size()+i];
			channelsz[i][j] = z[j*ip.labels.size()+i];
		}
	}

#ifdef _DEBUGG

	std::cout << "Streams: \n\n";

	for (int i = 0; i < ip.labels.size(); i++){
		std::cout << "Label: " << ip.labels[i];

		std::cout << "\n X:";
		for (int j = 0; j < ip.numFrames; j++){
			std::cout << " " << channelsx[i][j] << "\n";
		}
		std::cout << "\n Y:";
		for (int j = 0; j < ip.numFrames; j++){
			std::cout << " " << channelsy[i][j] << "\n";
		}
		std::cout << "\n Z:";
		for (int j = 0; j < ip.numFrames; j++){
			std::cout << " " << channelsz[i][j] << "\n";
		}
		std::cout << "\n";
	}
#endif
	//VARIANT ** vx = new VARIANT*[ip.labels.size()];
	//VARIANT ** vy = new VARIANT*[ip.labels.size()];
	//VARIANT ** vz = new VARIANT*[ip.labels.size()];

	//for (int i = 0; i < ip.labels.size(); i++){
	//	vx[i] = new VARIANT[ip.numFrames];
	//	vy[i] = new VARIANT[ip.numFrames];
	//	vz[i] = new VARIANT[ip.numFrames];
	//}

	//for (int i = 0; i < ip.labels.size(); i++){
	//	for (int j = 0; j < ip.numFrames; j++){
	//		VariantInit(&vx[i][j]);
	//		VariantInit(&vy[i][j]);
	//		VariantInit(&vz[i][j]);
	//	}
	//}

	//for (int i = 0; i < ip.labels.size(); i++){
	//	for (int j = 0; j < ip.numFrames; j++){
	//		vx[i][j].fltVal = channelsx[i][j];
	//		vy[i][j].fltVal = channelsy[i][j];
	//		vz[i][j].fltVal = channelsz[i][j];
	//	}
	//}

	VARIANT v;
	VariantInit(&v);
	VariantChangeType(&v, &v, 0, VT_ARRAY);

	SAFEARRAY* s;
	CreateSafeArray(&s, channelsx[0], ip.numFrames);

	v.parray = s;
	_variant_t vt = _variant_t();

	vt.Attach(v);
	//vt.ChangeType(VT_ARRAY);
	

	for (int i = 0; i < ip.labels.size(); i++){
		std::cout << p->SetPointDataEx(i, 0, 0, v); // X
		std::cout << p->SetPointDataEx(i, 1, 0, v); // Y
		std::cout << p->SetPointDataEx(i, 2, 0, v); // Z
	}

	p->SaveFile(bStrFile, 1);

	flush(std::cout);
	Sleep(100000);
	}
	CoUninitialize();
	return 0;
}

void CreateSafeArray(SAFEARRAY** s, float * data, int len)
{
	SAFEARRAYBOUND  Bound;
	Bound.lLbound = 0;
	Bound.cElements = len;

	*s = SafeArrayCreate(VT_R4, 1, &Bound);

	float HUGEP *pdFreq;
	HRESULT hr = SafeArrayAccessData(*s, (void HUGEP* FAR*)&pdFreq);
	if (SUCCEEDED(hr))
	{
		// copy sample values from data[] to this safearray
		for (DWORD i = 0; i < len; i++)
		{
			*pdFreq++ = data[i];
		}
		SafeArrayUnaccessData(*s);
	}
}

