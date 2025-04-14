using UnityEngine;

public class theEnergyBar : MonoBehaviour
{
    public Transform energyBar, hintBar;
    public AudioSource croakSFX;
    public bool isActive = false; // Indicates if the energy bar is being used
    public float energy = 1f, hint = 1f, prevLvl = 1f; // Current energy level (0 to 1)
    private float croakPower = 0.05f;
    private float croakCooldown = 0.33f;
    private float croakStartTime = 0f;
    private bool croaking = false; // Indicates if the croak action is in progress
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void setActive(bool active)
    {
        isActive = active; // Set the active state of the energy bar
    }
    public bool useEnergy(float amount)
    {
        if (energy-amount < 0f) return false; // Not enough energy
        energy -= amount; // Decrease energy by the specified amount
        hint -= amount; // Decrease hint by the same amount
        return true; // Energy used successfully
    }
    public void slideEnergy(float percent){
        energy = hint + (prevLvl-hint)*(1f-percent);
    }
    public bool useHint(float amount){
        if (energy-amount < 0f) return false; // Not enough energy for hint
        hint = energy-amount;
        prevLvl = energy; // Store the previous energy level
        return true; // Hint used successfully
    }
    public void gainEnergy(float amount)
    {
        energy += amount; // Increase energy by the specified amount
        if (energy > 1f) energy = 1f; // Clamp energy to a maximum of 1
        hint = energy; // Set hint to the current energy level
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        energyBar.localScale = new Vector3(1f, energy, 1f); // Update the energy bar scale based on current energy
        hintBar.localScale = new Vector3(1f, hint, 1f); // Update the hint bar scale based on current hint
        // Listen for RMB to croak (gain energy)
        if (Input.GetMouseButtonDown(1) && !isActive){
            croakStartTime = Time.time; // Record the time when the croak starts
            gainEnergy(croakPower); // Gain energy when croaking
            setActive(true); // Set the energy bar to active state
            croaking = true; // Set the croaking state to true
            if (croakSFX != null) croakSFX.Play(); // Play the croak sound effect
        }
        // Give control back when done croaking
        if (Time.time - croakStartTime >= croakCooldown && croaking){
            croaking = false; // Reset the croaking state
            setActive(false); // Set the energy bar to inactive state
        }
    }
}
