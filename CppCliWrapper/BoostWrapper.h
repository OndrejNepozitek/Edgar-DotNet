// BoostWrapper.h

#pragma once

using namespace System;

namespace CppCliWrapper {

	public ref class BoostWrapper
	{
	public:
		static array<array<int>^>^ GetFaces(int vertices_count, array<Tuple<int, int>^>^ edges);
	};
}
