using System.Collections;
using UnityEngine;

namespace ProjectRenaissance
{
    public sealed class Card : MonoBehaviour
    {
        public enum Suits { Clubs, Spades, Hearts, Diamonds }
        public enum Faces { Ace, Jack, King, Queen, None }

        [Header("State")]
        public int Value;
        public Faces Face;
        public Suits Suit;

        public CardEvent Revealed = new CardEvent();
        public CardEvent Left = new CardEvent();
        public CardEvent Arrived = new CardEvent();

        Rigidbody _rigidbody;
        Vector3 _velocityLimit;
        string _cachedCardName;
        bool _isRevealed = true;
        bool _isMidair = true;

        public bool IsRevealed
        {
            get
            {
                return _isRevealed;
            }
            set
            {
                bool old = _isRevealed;
                _isRevealed = value;

                if (old != value && value)
                    Revealed.Invoke(this);
            }
        }
        public string CardName
        {
            get
            {
                if (_cachedCardName == null)
                {
                    if (Face == Faces.None)
                        _cachedCardName = Value.ToString();
                    else
                        _cachedCardName = Face.ToString();
                }

                return _cachedCardName;
            }
        }

        void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _velocityLimit = new Vector3(0, -6f, 0);
            transform.localScale *= 1.25f;    
        }
        void FixedUpdate()
        {
            if (_rigidbody.velocity.y < _velocityLimit.y)
                _rigidbody.velocity = _velocityLimit;
        }
        void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.name == "Table")
            {
                _isMidair = false;
                _rigidbody.isKinematic = true;
            }
            else if (collision.collider.gameObject.layer.Equals(gameObject.layer))
            {
                Card card = collision.collider.GetComponent<Card>();

                if (card != null && !card._isMidair)
                {
                    _isMidair = false;
                    _rigidbody.isKinematic = true;
                }
            }
        }
        void OnCollisionStay(Collision collision)
        {
            if (collision.collider.name == "Table")
            {
                _isMidair = false;
                _rigidbody.isKinematic = true;
            }
            else if (collision.collider.gameObject.layer.Equals(gameObject.layer))
            {
                Card card = collision.collider.GetComponent<Card>();

                if (card != null && !card._isMidair)
                {
                    _rigidbody.isKinematic = true;
                    _isMidair = false;
                }
            }
        }

        public void RiseAndReveal()
        {
            StartCoroutine(AnimateRise(true));
        }
        public void RiseAndFlyTo(Vector3 targetPosition, Quaternion targetRotation)
        {
            StartCoroutine(AnimateRiseAndFlyTo(targetPosition, targetRotation));
            Left.Invoke(this);
        }

        IEnumerator AnimateRiseAndFlyTo(Vector3 targetPosition, Quaternion targetRotation)
        {
            Vector3 up = transform.position + new Vector3(0, 0.3f);
            float elapsedTime = 0;

            _rigidbody.isKinematic = true;

            while (up.y - transform.position.y > 0.1f)
            {
                elapsedTime += Time.deltaTime;
                transform.position = Vector3.Lerp(transform.position, up, elapsedTime / 0.5f);
                yield return new WaitForFixedUpdate();
            }

            StartCoroutine(AnimateFlyTo(targetPosition, targetRotation));
            yield break;
        }
        IEnumerator AnimateRise(bool doFlip)
        {
            _isMidair = true;
            Vector3 targetY = new Vector3(transform.localPosition.x, transform.localPosition.y + 0.8f, transform.localPosition.z);
            float elapsedTime = 0;

            while (targetY.y - transform.localPosition.y > 0.1f)
            {
                elapsedTime += Time.deltaTime;
                transform.localPosition = Vector3.Lerp(transform.localPosition, targetY, elapsedTime / 1f);
                yield return new WaitForFixedUpdate();
            }

            if (doFlip)
            {
                _rigidbody.isKinematic = false;
                StartCoroutine(AnimateFlip());
            }

            yield break;
        }
        IEnumerator AnimateFlyTo(Vector3 targetPosition, Quaternion targetRotation)
        {
            _isMidair = true;
            Vector3 initialPosition = transform.position;
            Quaternion initialRotation = transform.rotation;

            if (!IsRevealed)
                targetRotation = Quaternion.Euler(targetRotation.eulerAngles.x, targetRotation.eulerAngles.y, transform.rotation.eulerAngles.z);

            float animationTime = 0;

            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                animationTime += Time.deltaTime;
                transform.rotation = Quaternion.Lerp(initialRotation, targetRotation, animationTime / 1f);
                transform.position = Vector3.Lerp(initialPosition, targetPosition, animationTime / 1f);
                yield return new WaitForFixedUpdate();
            }

            transform.rotation = targetRotation;
            transform.position = targetPosition;
            _rigidbody.isKinematic = false;

            Arrived.Invoke(this);
            yield break;
        }
        IEnumerator AnimateFlip()
        {
            float elapsedTime = 0;

            while (transform.rotation.z > 0)
            {
                elapsedTime += Time.deltaTime;
                transform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(180, 0, elapsedTime / 0.3f));
                yield return new WaitForFixedUpdate();
            }

            IsRevealed = true;
            _rigidbody.isKinematic = false;
            yield break;
        }
    }
}