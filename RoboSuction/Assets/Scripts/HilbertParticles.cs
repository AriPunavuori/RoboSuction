using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class HilbertParticles : MonoBehaviour {
    
    [Range(1, 8)]
    public int gridComplexity = 3;
    public float dSpeed = 100f;

    // Hilbert implementation assumes this is power of 2
    int n;
    ParticleSystem ps;
    ParticleSystem.Particle[] particles;
    int newParticlesToAdd = 0;

    void Emit(int n) {
        newParticlesToAdd += n;
    }
    Vector3 RandomizeParticlePosition() {
        var d = Random.Range(0, n*n - 1);
        int x = 0;
        int y = 0;
        d2xy(n, d, ref x, ref y);
        //return new Vector3(2f, 0, 2f);
        return new Vector3(x, 0, y);
    }
    ParticleSystem.Particle makeParticle(Vector3 position) {
        ParticleSystem.Particle r = new ParticleSystem.Particle();
        //r.angularVelocity = part_angularVelocity;
        //r.color = part_color;
        r.remainingLifetime = 2f;// part_lifetime;
        r.position = position;
        //r.rotation = part_rotation;
        //r.size = part_size;
        r.startLifetime = 2f; // part_startLifetime;
        //r.velocity = part_velocity;

        return r;
    }

    void Awake() {
        n = 2 << gridComplexity;
        ps = GetComponent<ParticleSystem>();
        if (particles == null || particles.Length < ps.main.maxParticles)
            particles = new ParticleSystem.Particle[ps.main.maxParticles];

    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space))
            Emit(1);
        if (newParticlesToAdd > 0) {
            ps.Emit(newParticlesToAdd);
        }
        int count = ps.GetParticles(particles);
        for (int i = 0; i < count-newParticlesToAdd; i++) {
            var p = particles[i].position;
            var x1 = Mathf.RoundToInt(p.x);
            var y1 = Mathf.RoundToInt(p.z);
            int d1 = xy2d(n, x1, y1);
            // find the other point of the current curve segment
            int d0 = d1 - 1, d2 = d1 + 1;
            int x0 = 0, y0 = 0, x2 = 0, y2 = 0;
            d2xy(n, d0, ref x0, ref y0);
            d2xy(n, d2, ref x2, ref y2);
            bool d0closer = (p.x - x0) * (p.x - x0) + (p.z - y0) * (p.z - y0) <
                (x2 - p.x) * (x2 - p.x) + (y2 - p.z) * (y2 - p.z);
            var to = d0closer ? new Vector3(x1, 0, y1) : new Vector3(x2, 0, y2);
            particles[i].position = Vector3.MoveTowards(p, to, Time.deltaTime * dSpeed);
        }
        for (int i=count-newParticlesToAdd; i<count; i++) {
            particles[i].position = RandomizeParticlePosition();
        }
        newParticlesToAdd = 0;
        //while (newParticlesToAdd > 0) {
        //    particles[count] = makeParticle(RandomizeParticlePosition());
        //    count++;
        //    newParticlesToAdd--;
        //}
        ps.SetParticles(particles, count);
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
