using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class SimpleCollectorAgent : Agent
{
    [Tooltip("The platform to be moved around")]
    public GameObject goal;

    private Vector3 startPosition;
    private Vector3 startCoinPosition;
    private float before_distance;
    private SimpleCharacterController characterController;
    new private Rigidbody rigidbody;

    [SerializeField] private Material winMaterial;
    [SerializeField] private Material _winMaterial;
    [SerializeField] private Material hitCMaterial;
    [SerializeField] private Material hitBMaterial;
    [SerializeField] private Material startMaterial;
    [SerializeField] private Material farMaterial;
    [SerializeField] private MeshRenderer platformRender;
    [SerializeField] private float errorDistance = 2;
    [SerializeField] private Transform targetTransform;
    private float startDistance;
    /*    private float steps = 0 ;*/

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(targetTransform.position);
    }
    /// <summary>
    /// Called once when the agent is first initialized
    /// </summary>
    public override void Initialize()
    {

        startPosition = transform.position;

        startCoinPosition = goal.transform.position;


        characterController = GetComponent<SimpleCharacterController>();
        rigidbody = GetComponent<Rigidbody>();
        startDistance = Vector3.Distance(transform.position, goal.transform.position);
        before_distance = startDistance;
    }

    /// <summary>
    /// Called every time an episode begins. This is where we reset the challenge.
    /// </summary>
    public override void OnEpisodeBegin()
    {

        // Reset agent position, rotation
        transform.position = startPosition + new Vector3(Random.Range(-12.0f, 10.0f), 0f, Random.Range(10.5f, 15.5f));

/*        transform.position = startPosition;*/


        transform.rotation = Quaternion.Euler(Vector3.up * Random.Range(0f, 360f));

        rigidbody.velocity = Vector3.zero;

        /*        goal.transform.position = startCoinPosition + new Vector3(Random.Range(-15.0f, 16.0f), 0f, Random.Range(-20.0f, -16.0f));*/

        goal.transform.position = startCoinPosition;

        // Reset platform position (5 meters away from the agent in a random direction)

        // Reset the color of the platform

        // Reset startDistance and steps
        startDistance = Vector3.Distance(transform.position, goal.transform.position);
        before_distance = startDistance;
/*        platformRender.material = startMaterial;*/
    }

    /// <summary>
    /// Controls the agent with human input
    /// </summary>
    /// <param name="actionsOut">The actions parsed from keyboard input</param>
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Read input values and round them. GetAxisRaw works better in this case
        // because of the DecisionRequester, which only gets new decisions periodically.
        int vertical = Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));
        int horizontal = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
        bool jump = Input.GetKey(KeyCode.Space);

        // Convert the actions to Discrete choices (0, 1, 2)
        ActionSegment<int> actions = actionsOut.DiscreteActions;
        actions[0] = vertical >= 0 ? vertical : 2;
        actions[1] = horizontal >= 0 ? horizontal : 2;
        actions[2] = jump ? 1 : 0;
    }

    /// <summary>
    /// React to actions coming from either the neural net or human input
    /// </summary>
    /// <param name="actions">The actions received</param>
    public override void OnActionReceived(ActionBuffers actions)
    {
        before_distance = Vector3.Distance(goal.transform.position, transform.position);
        /*print("action_counter: "+ action_counter.ToString());
        print("threhold: "+(startDistance - action_counter * 0.001f).ToString());
        print(Vector3.Distance(goal.transform.position, transform.position));
        action_counter++;*/
        // Punish and end episode if the agent strays too far
        if (Vector3.Distance(goal.transform.position, transform.position) > (startDistance + errorDistance))
        {
            AddReward(-6f);
            platformRender.material = farMaterial;
            EndEpisode();
            
        }
        /*if (Vector3.Distance(goal.transform.position, transform.position) < startDistance - action_counter*0.001f)
        {
            AddReward(0.002f);
        }*/
        //AddReward(-0.001f);
/*        steps++;
        Debug.Log(steps);*/
        // Convert actions from Discrete (0, 1, 2) to expected input values (-1, 0, +1)
        // of the character controller
        float vertical = actions.DiscreteActions[0] <= 1 ? actions.DiscreteActions[0] : -1;
        float horizontal = actions.DiscreteActions[1] <= 1 ? actions.DiscreteActions[1] : -1;
        bool jump = actions.DiscreteActions[2] > 0;

        characterController.ForwardInput = vertical;
        characterController.TurnInput = horizontal;
        characterController.JumpInput = jump;
        if(Vector3.Distance(goal.transform.position, transform.position) < before_distance)
        {
            AddReward(0.005f);
        }
        else
        {
            AddReward(-0.01f);
        }
    }

    /// <summary>
    /// Respond to entering a trigger collider
    /// </summary>
    /// <param name="other">The object (with trigger collider) that was touched</param>
/*    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("123123121");
        // If the other object is a collectible, reward and end episode
        if (other.tag == "collectible")
        {
            AddReward(12f);
            platformRender.material = winMaterial;
            EndEpisode();
        }
*//*        else if (other.tag == "Building")
        {
            AddReward(-8f);
            platformRender.material = hitBMaterial;

            EndEpisode();
        }
        else if (other.tag == "Characters")
        {
            AddReward(-8f);
            platformRender.material = hitCMaterial;

            EndEpisode();
        }*//*
    }*/


    private void OnCollisionEnter(Collision collision)
    {
 
        if (collision.collider.tag == "collectible" && Vector3.Distance(startCoinPosition, transform.position) < 2)
        {
            AddReward(6f);
            platformRender.material = _winMaterial;
            goal.transform.position = startCoinPosition + new Vector3(Random.Range(-13.0f, 14.0f), 0f, Random.Range(-19.0f, -13.0f));
            startDistance = Vector3.Distance(transform.position, goal.transform.position);
            before_distance = startDistance;
/*            EndEpisode();*/

        }
        else if(collision.collider.tag == "collectible")
        {
            AddReward(12f);
            platformRender.material = winMaterial;
            EndEpisode();
        }
        else if (collision.collider.tag == "Building")
        {
            AddReward(-5f);
            platformRender.material = hitBMaterial;
            /*            EndEpisode();*/
        }
        else if (collision.collider.tag == "Characters")
        {
            AddReward(-4f);
            platformRender.material = hitCMaterial;
            /*            EndEpisode();*/
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.tag == "Building")
        {
            AddReward(0.5f);
            platformRender.material = startMaterial;
        }
        else if (collision.collider.tag == "Characters")
        {
            AddReward(0.5f);
            platformRender.material = startMaterial;
        }
    }


}
