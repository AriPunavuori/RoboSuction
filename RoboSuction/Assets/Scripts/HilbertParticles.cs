using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class HilbertParticles : MonoBehaviour {
    ParticleSystem ps;

    ParticleSystem.Particle[] particles;

    // Hilbert implementation assumes this is power of 2?
    public int n = 16;

    public float dSpeed = 100f;

    void Awake() {
        ps = GetComponent<ParticleSystem>();
        if (particles == null || particles.Length < ps.main.maxParticles)
            particles = new ParticleSystem.Particle[ps.main.maxParticles];

    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space))
            ps.Emit(1);

        int count = ps.GetParticles(particles);
        for (int i = 0; i < count; i++) {
            var p = particles[i].position;
            int x1 = Mathf.RoundToInt(p.x);
            int y1 = Mathf.RoundToInt(p.z);
            int d1 = xy2d(n, x1, y1);
            // find the other point of the current curve segment
            int d0 = d1 - 1, d2 = d1 + 1;
            int x0 = 0, y0 = 0, x2 = 0, y2 = 0;
            d2xy(n, d0, ref x0, ref y0);
            d2xy(n, d2, ref x2, ref y2);
            bool d0closer = Mathf.Abs(x1 - x0) + Mathf.Abs(y1 - y0) <
                Mathf.Abs(x2 - x1) + Mathf.Abs(y2 - y1);
            var from = d0closer ? new Vector3(x0, 0, y0) : new Vector3(x1, 0, y1);
            var to = d0closer ? new Vector3(x1, 0, y1) : new Vector3(x2, 0, y2);
            var dp = (to - from).normalized * Time.deltaTime * dSpeed;
            particles[i].position = p + dp;
        }
        ps.SetParticles(particles);
    }

    // https://www.reddit.com/r/learnprogramming/comments/3lb1gf/hilbert_curve_in_c/

    //convert (x,y) to d
    int xy2d(int n, int x, int y) {
        int rx, ry, s, d = 0;
        for (s = n / 2; s > 0; s /= 2) {

            rx = System.Convert.ToInt32(((x & s) > 0));
            ry = System.Convert.ToInt32((y & s) > 0);
            d += s * s * ((3 * rx) ^ ry);
            rot(s, ref x, ref y, rx, ry);
        }
        return d;
    }

    //convert d to (x,y)
    void d2xy(int n, int d, ref int x, ref int y) {
        int rx, ry, s, t = d;
        x = y = 0;
        for (s = 1; s < n; s *= 2) {
            rx = 1 & (t / 2);
            ry = 1 & (t ^ rx);
            rot(s, ref x, ref y, rx, ry);
            x += s * rx;
            y += s * ry;
            t /= 4;
        }
    }

    //rotate/flip a quadrant appropriately
    void rot(int n, ref int x, ref int y, int rx, int ry) {
        if (ry == 0) {
            if (rx == 1) {
                x = n - 1 - x;
                y = n - 1 - y;
            }

            //Swap x and y
            int t = x;
            x = y;
            y = t;
        }
    }
}
