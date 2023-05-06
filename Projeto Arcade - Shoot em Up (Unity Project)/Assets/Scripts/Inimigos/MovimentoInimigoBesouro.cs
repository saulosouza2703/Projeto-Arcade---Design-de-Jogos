using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimentoInimigoBesouro : MonoBehaviour
{
    public GameObject besouro, bosta;
    // movimento
    public float velocidadeMovimento = 2.0f, velocidadeRotacao = 2.5f, velocidadeRotacaoBosta = 2.0f;
    [Range (0.0f, 0.4f)]public float anguloRotacaoBosta;
    private float rotZ;
    private bool mudaDirecao = true;
    private float contadorCooldown;
    [Range(0.0f, 2.0f)] public float cooldownMudaDirecao = 2.0f;

    private void Start()
    {
        rotZ = besouro.transform.rotation.z;
    }
    void Update()
    {
        Movimento();
    }
     private void Movimento()
    {
        //  rotacao bosta
        bosta.transform.Rotate(velocidadeRotacaoBosta * Time.deltaTime, 0, 0, Space.Self);
        // direcao
        besouro.transform.Translate(0, velocidadeMovimento * Time.deltaTime, 0, Space.Self);
        // rotacao
        Utilidades.CalculaCooldown(contadorCooldown);
        contadorCooldown = Utilidades.CalculaCooldown(contadorCooldown);
        
        if (mudaDirecao)
        {
            besouro.transform.Rotate(0, 0, velocidadeRotacao * Time.deltaTime, Space.Self);
        }
        if (contadorCooldown == 0 && mudaDirecao == true)
        {
            mudaDirecao = false;
            contadorCooldown = cooldownMudaDirecao;
        }
        if (!mudaDirecao)
        {
            besouro.transform.Rotate(0, 0, - velocidadeRotacao * Time.deltaTime, Space.Self);
        }
        if (contadorCooldown == 0 && mudaDirecao == false)
        {
            mudaDirecao = true;
            contadorCooldown = cooldownMudaDirecao;
        }
    }
}
