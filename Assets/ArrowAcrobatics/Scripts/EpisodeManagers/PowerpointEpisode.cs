using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerpointEpisode : GenericEpisode
{
    public GameObject _screen = null;
    public List<Material> _slides = new List<Material>();
    private int _currentSlide = 0;
    private Renderer _rend = null;

    void Awake() {
        GetRenderer();
    }

    void OnEnable() {
        GetRenderer();
        UpdateSlide();
    }

    void OnDisable() {

    }

    public override void OnLaunch() {
        UpdateSlide();
    }


    public override bool next() {
        Debug.Log("powerpoint next");
        if(_currentSlide + 1 >= _slides.Count) {
            return false;
        } 

        _currentSlide++;
        UpdateSlide();

        return true;
    }

    public override bool prev() {
        Debug.Log("powerpoint next");
        if(_currentSlide - 1 < 0) {
            return false;
        }

        _currentSlide--;
        UpdateSlide();

        return true;
    }

    void GetRenderer() {
        if(_rend == null && _screen != null) {
            _rend = _screen.GetComponent<Renderer>();
        }
    }

    void UpdateSlide() {
        Debug.Log("update slide to " + _currentSlide.ToString());
        if(_rend != null && _slides != null) {
            _rend.material = _slides[_currentSlide];
        }
    }
}
