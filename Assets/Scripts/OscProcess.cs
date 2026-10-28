using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using extOSC;


public class OscProcess : MonoBehaviour
{

    public int photoInMin = 0;
    public int photoInMax = 1023;
    public float photoOutMin = 0.0f;
    public float photoOutMax = 1.0f;

    public OSCReceiver oscReceiver;
    public OSCTransmitter oscTransmitter;
    public AsteroidDodger asteroidDodger;

    void TraiterMessageAlpha(OSCMessage message)
    {
        // Traiter un entier et l’appliquer à une propriété (flux continu)

        // Récupérer la valeur du premier argument en taant qu’entier :
        int valeur = message.Values[0].IntValue;

        // Deboguer
        // Debug.Log("Reçu : " + message.Address + " " + valeur);

        // Ajustement de la valeur
        float ajustee = (
            ((float)valeur - photoInMin) / (photoInMax - photoInMin) * (photoOutMax - photoOutMin) + photoOutMin
        );

        // DESTINATION
    }

    // Start is called before the first frame update
    void Start()
    {
        oscReceiver.Bind("/photo", TraiterMessageAlpha);

    }

    // Update is called once per frame
    void Update()
    {
        if (asteroidDodger.GetShipIsVisible())
        {
            var oscMessage = new OSCMessage("/lumiere");
            oscMessage.AddValue(OSCValue.Int(1)); // Ajoute l’entier 1
            oscTransmitter.Send(oscMessage); // Envoyer le message 
}
        else
        {
            var oscMessage = new OSCMessage("/lumiere");
            oscMessage.AddValue(OSCValue.Int(0)); // Ajoute l’entier 0
            oscTransmitter.Send(oscMessage); // Envoyer le message 
}

    }
}