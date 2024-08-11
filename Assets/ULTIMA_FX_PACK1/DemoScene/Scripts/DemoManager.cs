using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class DemoObject
{
    [Serializable]
    public class ColorVariant
    {
        public Sprite UIButtonSprite;
        public GameObject Prefab;
    }
	
	public ViewMode ViewMode;
    public bool TrailMode;
    public ColorVariant[] ColorVariants;
}

public enum ViewMode { V_All, V_2DOnly }

public class DemoManager : MonoBehaviour
{
    [Header("Effects container:")]
    public DemoObject[] Effects;

    [Header("Ground textures:")]
    public Texture2D[] Textures;

    [Space(10)]
    public MeshRenderer GroundRenderer;
	public MeshRenderer GroundRenderer2D;
    public RotateCamera Camera;
	public GameObject Camera2D;

    public GameObject ColorVariantButtonPrototype;
    public Transform ColorVariantsContainer;

    public DemoButton ShowGround;
    public DemoButton CameraRotation;
    public DemoButton SlowMotion;
	public DemoButton PerspectiveButton;

    public Button NextEffectButton;
    public Button PreviousEffectButton;

    public GameObject NormalDescription;
    public GameObject TrailDescription;

    public Text CurrentEffectCounterText;
    public Text CurrentEffectNameText;

    private int _currentEffect = 0;
    private int _currentColorVariant = 0;
    private int _currentGround = 0;

    private List<GameObject> _currentColorButtons = new List<GameObject>();
	private bool _2dView;
	private bool _userSelected2d;
    private GameObject _trailEffect;

    private void Awake()
    {
        ShowGround.Button.onClick.AddListener(OnShowGroundClick);
        CameraRotation.Button.onClick.AddListener(OnCameraRotationClick);
        SlowMotion.Button.onClick.AddListener(OnSlowMotionClick);
        NextEffectButton.onClick.AddListener(() => ChangeCurrentEffect(1));
        PreviousEffectButton.onClick.AddListener(() => ChangeCurrentEffect(-1));
		PerspectiveButton.Button.onClick.AddListener(OnPerspectiveButtonClick);

        ShowGround.Text.text = "Ground " + _currentGround;
        ChangeCurrentEffect(0, false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            ChangeCurrentEffect(-1);

        if (Input.GetKeyDown(KeyCode.RightArrow))
            ChangeCurrentEffect(1);
			
		if (Input.GetKeyDown(KeyCode.UpArrow))
            ChangeCurrentColorVariant(_currentColorVariant + 1);

		if (Input.GetKeyDown(KeyCode.DownArrow))
            ChangeCurrentColorVariant(_currentColorVariant -1);

        if (Effects[_currentEffect].TrailMode)
        {
            if (Input.GetMouseButton(0))
            {
                var ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (_trailEffect == null)
                        _trailEffect = Instantiate(Effects[_currentEffect].ColorVariants[_currentColorVariant].Prefab);

                    _trailEffect.transform.position = hit.point + hit.normal;
                }
            }
            else
            {
                if (_trailEffect)
                    Destroy(_trailEffect);
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                var ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    var effect = Instantiate(Effects[_currentEffect].ColorVariants[_currentColorVariant].Prefab);
                    effect.transform.position = hit.point + hit.normal;
                    Destroy(effect.gameObject, 5);
                }
            }
        }
    }

    private void OnShowGroundClick()
    {
        _currentGround++;
        if (_currentGround >= Textures.Length)
            _currentGround = 0;

        if (Textures[_currentGround] == null)
        {
            GroundRenderer.enabled = false;
			GroundRenderer2D.enabled = false;
            ShowGround.Text.text = "Ground disabled";

        }
        else
        {
            ShowGround.Text.text = "Ground " + _currentGround;
            var block = new MaterialPropertyBlock();
            block.SetTexture("_MainTex", Textures[_currentGround]);
            GroundRenderer.SetPropertyBlock(block);
			GroundRenderer2D.SetPropertyBlock(block);
            GroundRenderer.enabled = true;
			GroundRenderer2D.enabled = true;
        }
    }

    private void OnCameraRotationClick()
    {
        Camera.ShouldRotate = !Camera.ShouldRotate;
    }
	
	private void OnPerspectiveButtonClick()
    {
        if(Effects[_currentEffect].ViewMode == ViewMode.V_2DOnly)
			return;
		
		if(_userSelected2d != _2dView)
			return;
		
		_userSelected2d = !_userSelected2d;
		ChangePerspective();
    }

    private void OnSlowMotionClick()
    {
        Time.timeScale = Time.timeScale == 1f ? 0.5f : 1f;
    }
	
	void ChangePerspective()
	{
		_2dView = !_2dView;
		
		Camera2D.gameObject.SetActive(_2dView);
		Camera.gameObject.SetActive(!_2dView);
		
		PerspectiveButton.Text.text = _2dView ? "2D Perspective" : "3D Perspective";
	}
	
	void UpdatePerspective()
	{
		var current = Effects[_currentEffect].ViewMode;		
		if((current == ViewMode.V_All && _2dView) || (current == ViewMode.V_2DOnly && !_2dView))
			ChangePerspective();
	}
	
    void ChangeCurrentEffect(int direction, bool updatePerspective = true)
    {
        _currentEffect += direction;
        if (_currentEffect < 0)
            _currentEffect = Effects.Length - 1;

        if (_currentEffect >= Effects.Length)
            _currentEffect = 0;

        if (Effects.Length == 0)
            return;

        _currentColorVariant = 0;

        CurrentEffectCounterText.text = string.Format("{0}/{1}", _currentEffect + 1, Effects.Length);

        NormalDescription.SetActive(!Effects[_currentEffect].TrailMode);
        TrailDescription.SetActive(Effects[_currentEffect].TrailMode);

        if (Effects[_currentEffect].ColorVariants.Length == 0)
            return;

        CurrentEffectNameText.text = Effects[_currentEffect].ColorVariants[_currentColorVariant].Prefab.name;

        foreach (var item in _currentColorButtons)
            Destroy(item);

        _currentColorButtons.Clear();

        int counter = 0;
        foreach(var item in Effects[_currentEffect].ColorVariants)
        {
            var button = Instantiate(ColorVariantButtonPrototype, ColorVariantsContainer);
            button.SetActive(true);
            button.GetComponent<Image>().sprite = item.UIButtonSprite;
            _currentColorButtons.Add(button);

            var index = counter;
            button.GetComponent<Button>().onClick.AddListener(() => 
            {
                ChangeCurrentColorVariant(index);
            });

            counter++;
        }

        _currentColorButtons[0].GetComponent<Button>().onClick.Invoke();
		PerspectiveButton.SetState(Effects[_currentEffect].ViewMode != ViewMode.V_2DOnly);
		
		if(_userSelected2d && _2dView)
			return;
			
		if(updatePerspective)
			UpdatePerspective();
    }

    void ChangeCurrentColorVariant(int index)
    {
		if(index >= Effects[_currentEffect].ColorVariants.Length)
			index = 0;
			
		if(index < 0)
			index = Effects[_currentEffect].ColorVariants.Length - 1;
		
        _currentColorVariant = index;
        CurrentEffectNameText.text = Effects[_currentEffect].ColorVariants[_currentColorVariant].Prefab.name;
		
		foreach (var btn in _currentColorButtons)
        		btn.transform.localScale = Vector3.one;
					
		_currentColorButtons[index].transform.localScale = Vector3.one * 1.5f;
    }
}
