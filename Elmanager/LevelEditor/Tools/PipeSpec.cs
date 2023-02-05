using System;
using System.Collections.Generic;
using Elmanager.Geometry;
using Elmanager.Lev;

namespace Elmanager.LevelEditor.Tools;

internal class PipeSpec
{
    public PipeSpec(Polygon pipeline, double radius, PipeMode mode, double appleDistance, int appleAmount)
    {
        Pipeline = pipeline;
        (Pipe, Apples) = UpdatePipe(radius, mode, appleDistance, appleAmount);
    }

    public List<LevObject> Apples { get; }

    public Polygon Pipe { get; }

    public Polygon Pipeline { get; }

    private List<LevObject> CalculateApples(double distance)
    {
        List<LevObject> apples = new List<LevObject>();
        double currentDistanceToApple = distance;
        for (int i = 0; i <= Pipeline.Vertices.Count - 2; i++)
        {
            Vector z = Pipeline[i + 1] - Pipeline[i];
            Vector zUnit = z.Unit();
            double currVectorLength = z.Length;
            if (currentDistanceToApple < currVectorLength)
            {
                double currVectorTrip = currentDistanceToApple;
                while (!(currVectorTrip > currVectorLength))
                {
                    apples.Add(new LevObject(Pipeline[i] + zUnit * currVectorTrip, ObjectType.Apple,
                        AppleType.Normal));
                    currVectorTrip += distance;
                }

                currentDistanceToApple = currVectorTrip - currVectorLength;
            }
            else
                currentDistanceToApple -= currVectorLength;
        }

        return apples;
    }

    private (Polygon p, List<LevObject> apples) UpdatePipe(double pipeRadius, PipeMode pipeMode, double appleDistance, int appleAmount)
    {
        var pipeLine = Pipeline;
        Polygon p = new Polygon();
        double angle = (pipeLine[1] - pipeLine[0]).Angle;
        p.Add(pipeLine[0] + new Vector(angle + 90) * pipeRadius);
        p.Add(pipeLine[0] - new Vector(angle + 90) * pipeRadius);
        for (int i = 1; i <= pipeLine.Vertices.Count - 2; i++)
        {
            angle = (pipeLine[i + 1] - pipeLine[i]).Angle;
            Vector point = GeometryUtils.FindPoint(pipeLine[i - 1], pipeLine[i], pipeLine[i + 1], -pipeRadius);
            p.Add(point);
        }

        p.Add(pipeLine.GetLastVertex() - new Vector(angle + 90) * pipeRadius);
        p.Add(pipeLine.GetLastVertex() + new Vector(angle + 90) * pipeRadius);
        for (int i = pipeLine.Vertices.Count - 2; i >= 1; i--)
        {
            Vector point = GeometryUtils.FindPoint(pipeLine[i - 1], pipeLine[i], pipeLine[i + 1], pipeRadius);
            p.Add(point);
        }

        p.UpdateDecomposition();
        List<LevObject> apples;
        switch (pipeMode)
        {
            case PipeMode.ApplesDistance:
                apples = CalculateApples(appleDistance);
                break;
            case PipeMode.ApplesAmount:
                double pipelineLength = 0.0;
                for (int i = 0; i <= pipeLine.Vertices.Count - 2; i++)
                    pipelineLength += (pipeLine[i + 1] - pipeLine[i]).Length;
                pipelineLength += 0.1;
                apples = CalculateApples(pipelineLength / (appleAmount + 1));
                break;
            case PipeMode.NoApples:
                apples = new List<LevObject>();
                break;
            default:
                throw new ArgumentException();
        }

        return (p, apples);
    }
}