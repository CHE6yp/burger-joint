﻿using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class PlayerController : MonoBehaviour {

    public static Player player;
    CharacterController contrl;
    public Camera cam;
    Vector3 camPos;
    //вторая камера чисто для видосов или типо того, потом надо вырезать
    public Camera cam2;
    public float speed = 1;

    //взаимодействие с предметами мышкой
    public bool choseUsable;
    public Usable currentUsable;

    void Start () 
    {
        player = GetComponent<Player>();
        contrl = GetComponent<CharacterController>();
        camPos = new Vector3(0, 15, -13);
	}

    void FixedUpdate()
    {
        float xMov = Input.GetAxis("Horizontal");
        float zMov = Input.GetAxis("Vertical");
        Vector3 movHorizontal = Vector3.right * xMov;
        Vector3 movVertical = Vector3.forward * zMov;

        Vector3 velocity = (movHorizontal + movVertical).normalized * speed;
        if (!player.seating)
        {
            transform.LookAt(transform.position + velocity);
            contrl.Move(velocity * Time.fixedDeltaTime);
        }

        //cam.transform.position = gameObject.transform.position + camPos;

        //НАДО ОТДЕЛЬНЫЙ КОНТРОЛЛЕР ДЛЯ АНИМАЦИИ
        if (velocity != new Vector3(0, 0, 0))
            GetComponent<Animator>().SetBool("move", true);
        else
            GetComponent<Animator>().SetBool("move", false);

    }

    void Update()
    {

        Camera.main.transform.Translate(Vector3.forward * Input.GetAxis("Mouse ScrollWheel")*3, Space.Self);
        if (Input.mousePosition.x > Screen.width - 20)
            Camera.main.transform.Translate(Vector3.right*0.5f);
        if (Input.mousePosition.x < 20)
            Camera.main.transform.Translate(Vector3.left*0.5f);
        if (Input.mousePosition.y > Screen.height - 20)
            Camera.main.transform.Translate(Vector3.forward * 0.5f,Space.World);
        if (Input.mousePosition.y < 20)
            Camera.main.transform.Translate(Vector3.back * 0.5f,Space.World);

        if (!EventSystem.current.IsPointerOverGameObject())
        {
            //over object text
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 10);
            

            if (!choseUsable)
            {
                if (Physics.Raycast(ray, out hit))
                {

                    if (Input.GetMouseButtonDown(1))
                    {
                        Debug.Log(player);
                        player.SetDestination(hit.point + new Vector3(0, 0.5f, 0));
                        Debug.Log("Clack");
                    }
                    if (hit.transform.GetComponent<Usable>() != null)
                    {
                        currentUsable = hit.transform.GetComponent<Usable>();

                        Transform objectHit = hit.transform;
                        //Debug.Log("Ray Hit! " + objectHit.name);
                        UIManager.uiManager.ShowOverText(hit.transform.gameObject);




                        //КЛИК!
                        if (Input.GetMouseButtonDown(0))
                        {
                            player.Triggered(hit.collider);
                            UIManager.uiManager.ContextDraw(hit.transform.gameObject.GetComponent<Usable>(), player);
                            choseUsable = true;
                        }

                    }
                    else
                    {
                        UIManager.uiManager.ContextClear();
                        player.UnTriggered(hit.collider);
                        currentUsable = null;
                        UIManager.uiManager.HideOverText();

                    }
                }

            }
            else
            {
                if (Physics.Raycast(ray, out hit))
                {
                    if (Input.GetMouseButtonDown(1))
                    {
                        player.SetDestination(hit.point + new Vector3(0, 0.5f, 0));
                        Debug.Log("Click");
                    }

                    if (hit.transform.GetComponent<Usable>() != null)
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            UIManager.uiManager.ContextClear();
                            player.Triggered(hit.collider);
                            UIManager.uiManager.ContextDraw(hit.transform.gameObject.GetComponent<Usable>(), player);
                            choseUsable = true;
                        }
                    }
                    else if (Input.GetMouseButtonDown(0))
                    {
                        choseUsable = false;

                        UIManager.uiManager.ContextClear();
                        player.UnTriggered(hit.collider);
                        currentUsable = null;
                        UIManager.uiManager.HideOverText();
                    }
                }
            }
        }
        //-------------

        //управление временем (для удобства, потом надо вырезать)
        if (Input.GetKeyDown(KeyCode.Alpha1))
            Time.timeScale = 1;
        if (Input.GetKeyDown(KeyCode.Alpha2))
            Time.timeScale = 2;
        if (Input.GetKeyDown(KeyCode.Alpha3))
            Time.timeScale = 3;
        if (Input.GetKeyDown(KeyCode.Alpha4))
            Time.timeScale = 0.5f;

        //смена камеры, для видосов
        if (Input.GetKeyDown(KeyCode.C))
        {
            cam.enabled = !cam.enabled;
            cam2.enabled = !cam2.enabled;
        }
    }
}
