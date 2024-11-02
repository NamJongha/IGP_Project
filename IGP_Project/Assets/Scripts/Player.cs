using Fusion;
using NetworkRigidbody2D = Fusion.Addons.Physics.NetworkRigidbody2D;

public class Player : NetworkBehaviour
{
    private NetworkRigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<NetworkRigidbody2D>();
    }
    public override void FixedUpdateNetwork()
    {
        if(GetInput<NetworkInputData_tutorial>(out var input))
        {
            
        }
    }

    void UpdateMovement(NetworkInputData_tutorial input)
    {
        if (input.GetButton(InputButton.LEFT))
        {

        }

        else if (input.GetButton(InputButton.RIGHT))
        {

        }
    }
}