﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    public FlockAgent agentPrefab;
    List<FlockAgent> agents = new List<FlockAgent>();
    public FlockBehavior behavior;
    public float scaredDistance;
    public float scaredDuration;
    public float spawnRadius;

    [Range(5, 500)]
    public int startingCount = 250;

    [Range(0f, 0.5f)]
    public float AgentDensity = 0.08f;

    [Range(1f, 100f)]
    public float driveFactor = 10f;
    [Range(1f, 100f)]
    public float maxSpeed = 5f;
    [Range(1f, 10f)]
    public float neighborRadius = 1.5f;
    [Range(0f, 1f)]
    public float avoidanceRadiusMultiplier = 0.5f;

    float squareMaxSpeed;
    float squareNeighborRadius;
    float squareAvoidanceRadius;
    public float SquareAvoidanceRadius { get { return squareAvoidanceRadius; } }

    // Start is called before the first frame update
    void Start()
    {
        startingCount = ((int)CONSTANTS.difficulty + 1) * 50;
        squareMaxSpeed = maxSpeed * maxSpeed;
        squareNeighborRadius = neighborRadius * neighborRadius;
        squareAvoidanceRadius = squareNeighborRadius * avoidanceRadiusMultiplier * avoidanceRadiusMultiplier;

        for (int i = 0; i < startingCount; i++)
        {
            FlockAgent newAgent = Instantiate(
                agentPrefab,
                Random.insideUnitCircle * spawnRadius * AgentDensity,
                Quaternion.Euler(Vector3.forward * Random.Range(0f, 360f)),
                transform
                );
            newAgent.name = "Agent " + i;
            newAgent.Initialize(this);
            agents.Add(newAgent);
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (FlockAgent agent in agents)
        {
            if (agent != null) {

                List<Transform> context = GetNearbyObjects(agent);

                //FOR DEMO ONLY
                // agent.GetComponentInChildren<SpriteRenderer>().color = Color.Lerp(Color.white, Color.red, context.Count / 6f);

                Vector2 move = behavior.CalculateMove(agent, context, this);
                move *= driveFactor;
                if (move.sqrMagnitude > squareMaxSpeed)
                {
                    move = move.normalized * maxSpeed;
                }
                agent.Move(move);
                updateExpression(agent);
            }
        }
    }


    List<Transform> GetNearbyObjects(FlockAgent agent)
    {   
        List<Transform> context = new List<Transform>();
        if (agent == null) return context;
        Collider2D[] contextColliders = Physics2D.OverlapCircleAll(agent.transform.position, neighborRadius);
        foreach (Collider2D c in contextColliders)
        {
            if (c != agent.AgentCollider)
            {
                context.Add(c.transform);
            }
        }
        return context;
    }

    void updateExpression(FlockAgent fa)
    {
        GameObject[] missiles = GameObject.FindGameObjectsWithTag("Explosion");
        float c = 0;
        float sumDist = 0;
        foreach (GameObject m in missiles)
        {
            sumDist += Vector2.Distance(fa.transform.position, m.transform.position);
            c++;
        }
        if (sumDist/c < scaredDistance)
        {
            fa.changeExpression(ExpressionGenerator.Emotion.Scared, scaredDuration);
        }
    }

}
