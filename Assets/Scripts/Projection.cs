using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Projection : MonoBehaviour {

    [SerializeField] private LineRenderer _line;

    int numPuntos = 0;

    public void DrawTrajectoryOnShoot(Proyectil ball)
    {
        StopAllCoroutines();
        _line.positionCount = 0;
        numPuntos = 0;
        StartCoroutine(DrawingTrajectory(ball, .05f));
    }

    IEnumerator DrawingTrajectory(Proyectil ball, float timeBetweenPoints)
    {
        while (ball.numCol <= 0)
        {
            numPuntos++;
            yield return new WaitForSeconds(timeBetweenPoints);
            _line.positionCount = numPuntos;
            _line.SetPosition(numPuntos - 1, ball.transform.position);
        }
    }
}