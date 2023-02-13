using System;
using UnityEngine;
using System.Collections.Generic;

public class LineSmoother
{
	private AnimationCurve curveX = new AnimationCurve();
	private AnimationCurve curveY = new AnimationCurve();
	private AnimationCurve curveZ = new AnimationCurve();
	public Vector3[] SmoothLine( Vector3[] inputPoints, float segmentSize )
	{
		curveX.keys = Array.Empty<Keyframe>();
		curveY.keys =  Array.Empty<Keyframe>();
		curveZ.keys =  Array.Empty<Keyframe>();	
		
		//create keyframe sets
		Keyframe[] keysX = new Keyframe[inputPoints.Length];
		Keyframe[] keysY = new Keyframe[inputPoints.Length];
		Keyframe[] keysZ = new Keyframe[inputPoints.Length];

		//set keyframes
		for( int i = 0; i < inputPoints.Length; i++ )
		{
			keysX[i] = new Keyframe( i, inputPoints[i].x );
			keysY[i] = new Keyframe( i, inputPoints[i].y );
			keysZ[i] = new Keyframe( i, inputPoints[i].z );
		}

		//apply keyframes to curves
		curveX.keys = keysX;
		curveY.keys = keysY;
		curveZ.keys = keysZ;

		//smooth curve tangents
		for( int i = 0; i < inputPoints.Length; i++ )
		{
			curveX.SmoothTangents( i, 0 );
			curveY.SmoothTangents( i, 0 );
			curveZ.SmoothTangents( i, 0 );
		}

		//list to write smoothed values to
		List<Vector3> lineSegments = new List<Vector3>();

		//find segments in each section
		for( int i = 0; i < inputPoints.Length; i++ )
		{
			//add first point
			lineSegments.Add( inputPoints[i] );

			//make sure within range of array
			if( i+1 < inputPoints.Length )
			{
				//find distance to next point
				float distanceToNext = Vector3.Distance(inputPoints[i], inputPoints[i+1]);

				//number of segments
				int segments = (int)(distanceToNext / segmentSize);

				//add segments
				for( int s = 1; s < segments; s++ )
				{
					//interpolated time on curve
					float time = ((float)s/(float)segments) + (float)i;

					//sample curves to find smoothed position
					Vector3 newSegment = new Vector3( curveX.Evaluate(time), curveY.Evaluate(time), curveZ.Evaluate(time) );

					//add to list
					lineSegments.Add( newSegment );
				}
			}
		}

		return lineSegments.ToArray();
	}

}
