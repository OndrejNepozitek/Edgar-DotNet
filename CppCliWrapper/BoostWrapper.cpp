#include "BoostWrapper.h"
#include "Stdafx.h"
#include <boost\graph\adjacency_list.hpp>
#include <boost\graph\boyer_myrvold_planar_test.hpp>
#include <boost\graph\planar_face_traversal.hpp>
#include <iostream>
#include <iterator>
#include <algorithm>
#include <map>
#include <vector>

using namespace System;

struct Vertex { int number; };

struct Edge { };

struct output_visitor : public boost::planar_face_traversal_visitor
{
	std::vector<std::vector<int>> faces;
	std::vector<int> last_face;

	void begin_face()
	{
		
	}

	void next_vertex(int v)
	{
		last_face.push_back(v);
	}

	void end_face()
	{
		faces.push_back(last_face);
		last_face.clear();
	}
};

array<array<int>^>^ CppCliWrapper::BoostWrapper::GetFaces(int vertices_count, array<Tuple<int, int>^>^ edges)
{
	using namespace boost;

	typedef adjacency_list
		< vecS,
		vecS,
		undirectedS,
		property<vertex_index_t, int>,
		property<edge_index_t, int>
		>
		graph;

	graph g(vertices_count);

	for each(auto edge in edges)
	{
		boost::add_edge(edge->Item1, edge->Item2, g);
	}

	// Initialize the interior edge index
	property_map<graph, edge_index_t>::type e_index = get(edge_index, g);
	graph_traits<graph>::edges_size_type edge_count = 0;
	graph_traits<graph>::edge_iterator ei, ei_end;
	for (tie(ei, ei_end) = boost::edges(g); ei != ei_end; ++ei)
		put(e_index, *ei, edge_count++);

	// Test for planarity - we know it is planar, we just want to 
	// compute the planar embedding as a side-effect
	typedef std::vector< graph_traits<graph>::edge_descriptor > vec_t;
	std::vector<vec_t> embedding(num_vertices(g));
	if (!boyer_myrvold_planarity_test(boyer_myrvold_params::graph = g,
		boyer_myrvold_params::embedding =
		&embedding[0]
	))
	{
		return nullptr;
	}

	output_visitor my_visitor;
	boost::planar_face_traversal(g, &embedding[0], my_visitor);

	array<array<int>^>^ faces = gcnew array<array<int>^>(static_cast<int>(my_visitor.faces.size()));

	for (int i = 0; i < my_visitor.faces.size(); i++)
	{
		auto face_raw = my_visitor.faces[i];
		auto face = gcnew array<int>(static_cast<int>(face_raw.size()));

		for (int j = 0; j < face_raw.size(); j++)
		{
			face[j] = static_cast<int>(face_raw[j]);
		}

		faces[i] = face;
	}

	return faces;
}
