using Nakama;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Auroraland
{
    public class EntranceSceneController : SceneController
    {
        enum Errors { Unknown = 0, EmptyEmailInput, EmptyPasswordInput, EmptyConfirmPasswordInput, PasswordsNotMatch, InvalidPassword, UserNotFound, UserRegisterInuse, AuthError, BadInput, InvalidEmail }

        public TransitDoor TransitDoor;

        [Header("UI")]
        [SerializeField]
        GameObject _authenticationCanvas;
        [SerializeField]
        GameObject _voiceCommandCanvas;
        [SerializeField]
        Toggle _rememberMeToggle;
        [SerializeField]
        Text _errorText;

        [Header("UI/Sign In")]
        [SerializeField]
        InputField _signInEmail;
        [SerializeField]
        InputField _signInPassword;
        [SerializeField]
        Button _signInTab;
        [SerializeField]
        Button _signInButton;

        [Header("UI/Sign Up")]
        [SerializeField]
        InputField _signUpEmail;
        [SerializeField]
        InputField _signUpPassword;
        [SerializeField]
        InputField _signUpConfirmPassword;
        [SerializeField]
        Button _signUpTab;
        [SerializeField]
        Button _signUpButton;

        readonly Dictionary<Errors, string> _errorMessages = new Dictionary<Errors, string>()
        {
            { Errors.EmptyEmailInput, "Please make sure to enter e-mail address.\n"},
            { Errors.EmptyPasswordInput, "Please make sure to enter password.\n"},
            { Errors.EmptyConfirmPasswordInput, "Please make sure to re-enter password.\n"},
            { Errors.InvalidEmail, "Please make sure to enter a valid e-mail address.\n"},
            { Errors.PasswordsNotMatch, "Please make sure that password and confirm password are matched.\n"},
            { Errors.InvalidPassword, "Passwords must contain at least 8 characters. Check that the password contains the appropriate amount of characters.\n"},
            { Errors.UserNotFound, "The e-mail address is not registered. Please select sign up panel to create an account.\n"},
            { Errors.UserRegisterInuse, "The e-mail address is registered already. Please use another e-mail address to register or go to sign up panel.\n"},
            { Errors.AuthError, "The e-mail or password you entered is incorrect. Please try again.\n"},
            { Errors.Unknown, "Your attempt was not successful. Please check your internet connection and try again.\n"}
        };

        void Start()
        {
            Assert.IsNotNull(_authenticationCanvas);
            Assert.IsNotNull(_voiceCommandCanvas);
            Assert.IsNotNull(_rememberMeToggle);
            Assert.IsNotNull(_errorText);

            Assert.IsNotNull(_signInEmail);
            Assert.IsNotNull(_signInPassword);
            Assert.IsNotNull(_signInTab);
            Assert.IsNotNull(_signInButton);

            Assert.IsNotNull(_signUpEmail);
            Assert.IsNotNull(_signUpPassword);
            Assert.IsNotNull(_signUpConfirmPassword);
            Assert.IsNotNull(_signUpTab);
            Assert.IsNotNull(_signUpButton);

            _signInButton.onClick.AddListener(SignIn);
            _signUpButton.onClick.AddListener(SignUp);

            bool rememberMePlayerPref = PlayerPrefs.GetInt("rememberMe") == 1;
            _rememberMeToggle.isOn = rememberMePlayerPref;

            if (rememberMePlayerPref)
            {
                _signInEmail.text = PlayerPrefs.GetString("nk.email");
                _signInPassword.text = PlayerPrefs.GetString("nk.password");
            }

            _rememberMeToggle.onValueChanged.AddListener((val) =>
            {
                PlayerPrefs.SetInt("rememberMe", val ? 1 : 0);
            });

            _signInEmail.onEndEdit.AddListener((text) =>
            {
                if (text.Length == 0)
                    _errorText.text = _errorMessages[Errors.EmptyEmailInput];

                PlayerPrefs.SetString("nk.email", text);
            });

            _signInPassword.onEndEdit.AddListener((text) =>
            {
                if (text.Length == 0)
                    _errorText.text = _errorMessages[Errors.EmptyPasswordInput];
                else if (text.Length < 8)
                    _errorText.text = _errorMessages[Errors.InvalidPassword];

                PlayerPrefs.SetString("nk.password", text);
            });

            _signUpConfirmPassword.onEndEdit.AddListener((text) =>
            {
                if (text.Length == 0)
                    _errorText.text = _errorMessages[Errors.EmptyConfirmPasswordInput];

                if (text.CompareTo(_signUpPassword.text) != 0)
                    _errorText.text = _errorMessages[Errors.PasswordsNotMatch];
            });

            _signInTab.onClick.AddListener(() =>
            {
                _errorText.text = "";
            });

            _signUpTab.onClick.AddListener(() =>
            {
                _errorText.text = "";
            });
        }

        public void SignIn()
        {
            _errorText.text = "";

            NKController.Instance.LoginByEmail(_signInEmail.text, _signInPassword.text, (bool hasSucceeded, INError error) =>
            {
                if (hasSucceeded)
                {
                    _authenticationCanvas.SetActive(false);
                    _voiceCommandCanvas.SetActive(false);
                    Home();
                }
                else
                    ParseErrorCode(error);
            });
        }

        public void SignUp()
        {
            _errorText.text = "";

            if (_signUpPassword.text.Equals(_signUpConfirmPassword.text))
            {
                NKController.Instance.RegisterByEmail(_signUpEmail.text, _signUpPassword.text, (bool hasSucceeded, INError error) =>
                {
                    if (hasSucceeded)
                    {
                        _authenticationCanvas.SetActive(false);
                        _voiceCommandCanvas.SetActive(false);
                        Home();
                    }
                    else
                        ParseErrorCode(error);
                });
            }
            else
            {
                _errorText.text = _errorMessages[Errors.PasswordsNotMatch];
            }
        }

        void ParseErrorCode(INError error)
        {
            switch (error.Code)
            {
                case ErrorCode.BadInput:
                    if (error.Message.Contains("Invalid email"))
                        _errorText.text += _errorMessages[Errors.InvalidEmail];
                    else if (error.Message.Contains("Email address is required"))
                        _errorText.text += _errorMessages[Errors.EmptyEmailInput];
                    else if (error.Message.Contains("Password must be longer than 8 characters"))
                        _errorText.text += _errorMessages[Errors.InvalidPassword];
                    break;
                case ErrorCode.UserNotFound:
                    _errorText.text += _errorMessages[Errors.UserNotFound];
                    break;
                case ErrorCode.UserRegisterInuse:
                    _errorText.text += _errorMessages[Errors.UserRegisterInuse];
                    break;
                case ErrorCode.AuthError:
                    _errorText.text += _errorMessages[Errors.AuthError];
                    break;
                default:
                    _errorText.text += _errorMessages[Errors.Unknown];
                    break;
            }
        }

        /*Called from Entrance Scene*/
        public override void Home()
        {
            StartCoroutine(EnterHome());
        }

        IEnumerator EnterHome()
        {
            TransitDoor.OpenDoor();
            yield return new WaitWhile(() => TransitDoor.IsAnimating);
            SceneManager.LoadScene(TransitionSceneName);
        }
    }
}
