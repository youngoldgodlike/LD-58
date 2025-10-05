using System;
using UnityEngine;
[Serializable]
public class CustomWait : CustomYieldInstruction {

    public float time;
    public Func<float> waitTime;
    
    public override bool keepWaiting {
        get {
            time += Time.deltaTime;
            if (waitTime() < time) {
                time = 0;
                return false;
            }
            return true;
        }
    }

    public CustomWait(Func<float> waitTime) {
        this.waitTime = waitTime;
    } 
}
