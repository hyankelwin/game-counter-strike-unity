using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(CharacterController))]
public class Controller : MonoBehaviour
{
    public float speed = 6.0f;
    private GameObject cameraFPS;
    private Vector3 moveDirection = Vector3.zero;
    private CharacterController controller;
    private float rotacaoX = 0;
    private float rotacaoY = 0.0f;

    public Rigidbody projectile;
    public float speedshooter = 100;
    // Start is called before the first frame update
    void Start()
    {
        cameraFPS = GetComponentInChildren(typeof(Camera)).transform.gameObject;
        cameraFPS.transform.localPosition = new Vector3(0,1,0);
        cameraFPS.transform.localRotation = Quaternion.identity;
        controller = GetComponent<CharacterController>();

        //Esconde o mouse;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Apenas movimenta o jogador se ele estiver no chão
        if (controller.isGrounded) {
            //Pega a direção da face à frente da camera
            Vector3 direcaoFrente = new Vector3(cameraFPS.transform.forward.x, 0, cameraFPS.transform.forward.z);
            //Pega a direção da face ao lado da camera
            Vector3 direcaoLado = new Vector3(cameraFPS.transform.right.x, 0, cameraFPS.transform.right.z);
            //Normaliza os valores para o máximo de 1, para que o jogador ande sempre na mesma velocidade
            direcaoFrente.Normalize();
            direcaoLado.Normalize();

            //Pega o valor das teclas para cima (1) e para baixo (-1)
            direcaoFrente = direcaoFrente * Input.GetAxis("Vertical");
            //Pega o valor das teclas para direita (1) e para baixo (-1)
            direcaoLado = direcaoLado * Input.GetAxis("Horizontal");

            //Soma a movimentação lateral com a movimentação para frente/trás
            Vector3 direcaoFinal = direcaoFrente + direcaoLado;
            
            if (direcaoFinal.sqrMagnitude > 1) {
                direcaoFinal.Normalize();
            } 
            
            //Apenas move nas direçoes X e Z
            moveDirection = new Vector3(direcaoFinal.x, 0, direcaoFinal.z);

            //Pular
            if (Input.GetButton(("Jump"))) {
                moveDirection.y = 0.8f;
            }

            //Multiplica pela velocidade que foi configurada no jogador
            moveDirection = moveDirection * speed;
        }
        //Faz o jogador ir para baixo (Gravidade)
        moveDirection.y -= 20.0f * Time.deltaTime;

        //Faz o movimento
        controller.Move(moveDirection * Time.deltaTime);

        cameraPrimeiraPessoa();

        if (Input.GetButtonDown("Fire1")) {
            // Debug.Log("Tiro");

            Rigidbody hitPlayer;
            hitPlayer = Instantiate(projectile, cameraFPS.transform.position, cameraFPS.transform.rotation) as Rigidbody;
            hitPlayer.velocity = cameraFPS.transform.TransformDirection(Vector3.forward * speedshooter);
        }
    }

    void cameraPrimeiraPessoa() {

        rotacaoX += Input.GetAxis("Mouse X") * 10.0f;
        rotacaoY += Input.GetAxis("Mouse Y") * 10.0f;

        rotacaoX = clampAngleFPS(rotacaoX, -360, 360);
        rotacaoY = clampAngleFPS(rotacaoY, -80, 80);

        Quaternion xq = Quaternion.AngleAxis(rotacaoX, Vector3.up);
        Quaternion yq = Quaternion.AngleAxis(rotacaoY, - Vector3.right);
        Quaternion q = Quaternion.identity * xq * yq;

        cameraFPS.transform.localRotation = Quaternion.Lerp(cameraFPS.transform.localRotation, q, Time.deltaTime * 10.0f);
    }

    float clampAngleFPS(float angulo, float min, float max) {
        if ( angulo < - 360 ) {
            angulo += 360;
        }

        if ( angulo > 360 ) {
            angulo -= 360;
        }

        return Mathf.Clamp(angulo, min, max);
    }
}
