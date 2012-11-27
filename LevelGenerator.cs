/*using System.Collections.Generic;
using System;

namespace Elmanager
{
	internal class LevelGenerator
	{
		
		private const double MaximumLevelSize = 188.0;
		private const double GroundDistanceFromDrivingPath = 1;
		private double MinLevelLength = 20;
		private double MaxLevelLength = 100; //Total length of all driving paths
		private double DifferentPathFrequency = 0.1; //[0,1] - How many "styles" the level will have relatively
		private double GravityAppleFrequency = 0;
		private List<DrivePath>[] DrivingPaths = new List<DrivePath>[1];
		private List<Model> Models;
		internal class Model
		{
			internal string Name;
			internal List<Vector> Ground;
			internal List<Level.LevelObject> Killers;
			internal Bound ConsecutiveLength;
			internal Bound GroundThickness;
			internal Bound RoofHeight;
			internal Bound RoofThickness;
			internal Bound Rotation; //[0,90] - How much the model can be rotated in relation to current gravity direction
			internal Bound RotationStep; //[0,90] - How much the model can be rotated in one step (vertex)
			internal Bound ScalingX;
			internal Bound ScalingY; //If ground is only two vertices, this is useless
			internal Bound SpeedRequirement; //Speed required to drive through this model (maybe this should be calculated)
			internal Bound DistanceFromDifferentModel; //Empty space between this model and a different one
			internal bool AllowApples; //Can this model have apples
			internal bool AllowGravityApples; //Can this model have gravity apples
			internal bool CanSplit;
			internal bool IsSingle; //Should this model be treated as a single edge while generating the level (e.g. loops are not just single)
			internal double GroundVariance; //How much the positions of the ground vertices can be randomized
			internal BoundInt SplitFrequency; //How many additional vertices must be generated between two ground vertices
			internal double SplitVariance; //How much the positions of the additional vertices can be randomized
			internal double RelativeFrequency; //[0,1] - Relative amount of this model in the level
		}
		internal struct Bound
		{
			public double Minimum;
			public double Maximum;
			static Random Rnd = new Random();
			internal double GetRandom()
			{
				return Minimum + Rnd.NextDouble() * (Maximum - Minimum);
			}
		}
		internal struct BoundInt
		{
			public int Minimum;
			public int Maximum;
			static Random Rnd = new Random();
			internal int GetRandom()
			{
				return Rnd.Next(Minimum, Maximum + 1);
			}
		}
		private struct DrivePath
		{
			private Vector Point;
			private Model Model;
			private Vector OrthogonalVector;
			private Vector Direction;
			private bool IsDivergingPoint;
			private bool IsEnd;
			private bool IsEmpty;
		}
		private enum GravityDirection
		{
			GravityUp = 1,
			GravityDown = 2,
			GravityLeft = 3,
			GravityRight = 4
		}
		internal Level Generate()
		{
			Level GeneratedLevel = new Level(new Polygon(), new Vector(), new Vector());
			Model Pipe = null;
			Pipe.Name = "Pipe";
			Pipe.AllowApples = true;
			Pipe.AllowGravityApples = false;
			Pipe.Ground = new List<Vector>();
			Pipe.Ground.Add(new Vector(0, 0));
			Pipe.Ground.Add(new Vector(3, 0));
			Pipe.GroundVariance = 0.5;
			Pipe.IsSingle = true;
			Pipe.Killers = new List<Level.LevelObject>();
			Pipe.GroundThickness.Maximum = 10.0;
			Pipe.RoofHeight.Maximum = 2.0;
			Pipe.RoofThickness.Maximum = 10.0;
			Pipe.Rotation.Maximum = 90.0;
			Pipe.ScalingX.Maximum = 5.0;
			Pipe.ScalingY.Maximum = 1.0;
			Pipe.DistanceFromDifferentModel.Minimum = 0.0;
			Pipe.GroundThickness.Minimum = 1.0;
			Pipe.RoofHeight.Minimum = 1.5;
			Pipe.RoofThickness.Minimum = 2.0;
			Pipe.ScalingX.Minimum = 0.5;
			Pipe.ScalingY.Minimum = 1.0;
			Pipe.SplitFrequency.Maximum = 5;
			Pipe.SplitFrequency.Minimum = 0;
			Pipe.SplitVariance = 0.2;
			Pipe.RelativeFrequency = 1.0;
			Pipe.CanSplit = false;
			Pipe.RotationStep.Minimum = 1;
			Pipe.RotationStep.Maximum = 90.0;
			Model NormalGround = null;
			NormalGround.Name = "Normal ground";
			NormalGround.AllowApples = true;
			NormalGround.AllowGravityApples = false;
			NormalGround.Ground = new List<Vector>();
			NormalGround.Ground.Add(new Vector(0, 0));
			NormalGround.Ground.Add(new Vector(3, 0));
			NormalGround.GroundVariance = 0.5;
			NormalGround.IsSingle = true;
			NormalGround.Killers = new List<Level.LevelObject>();
			NormalGround.GroundThickness.Maximum = 10.0;
			NormalGround.RoofHeight.Maximum = 10.0;
			NormalGround.RoofThickness.Maximum = 10.0;
			NormalGround.Rotation.Maximum = 30.0;
			NormalGround.ScalingX.Maximum = 5.0;
			NormalGround.ScalingY.Maximum = 1.0;
			NormalGround.DistanceFromDifferentModel.Maximum = 0.0;
			NormalGround.GroundThickness.Minimum = 1.0;
			NormalGround.RoofHeight.Minimum = 4.0;
			NormalGround.RoofThickness.Minimum = 2.0;
			NormalGround.ScalingX.Minimum = 0.5;
			NormalGround.ScalingY.Minimum = 1.0;
			NormalGround.Rotation.Minimum = 0.0;
			NormalGround.SplitFrequency.Maximum = 5;
			NormalGround.SplitFrequency.Minimum = 0;
			NormalGround.SplitVariance = 0.2;
			NormalGround.RelativeFrequency = 0.2;
			NormalGround.CanSplit = true;
			NormalGround.RotationStep.Minimum = 1;
			NormalGround.RotationStep.Maximum = 20;
			Models = new List<Model>();
			Models.Add(Pipe);
			Models.Add(NormalGround);
			Random Rnd = new Random();
			double TotalLength = MinLevelLength + Rnd.NextDouble() * (MaxLevelLength - MinLevelLength);
			double CurrentLength = 0.0;
			GeneratedLevel.Objects.Add(new Level.LevelObject(new Vector(0, 0), Level.ObjectType.Start, Level.AppleType.Normal, 0));
			double CurrentSpeed = 0.0;
			Vector CurrentDirection;
			if (Rnd.NextDouble() < 0.5)
			{
				CurrentDirection = new Vector(1, 0);
			}
			else
			{
				CurrentDirection = new Vector(-1, 0);
			}
			double CurrentModelLength = 0.0;
			while (CurrentLength < TotalLength)
			{
				
			}
			return GeneratedLevel;
		}
		private bool IsEnoughSpace(DrivePath D)
		{
			foreach (List<DrivePath> x in DrivingPaths)
			{
				if (x.Contains(D))
				{
					continue;
				}
				foreach (DrivePath z in x)
				{
					
				}
			}
			
			return true;
		}
		
	}
}*/