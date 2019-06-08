using UnityEngine;

namespace NG
{
    /// <summary>
    /// Reflection-style struct to temporarily store the values of a Rigidbody
    /// so that they can be restored after pausing.
    /// </summary>
    [System.Serializable]
    public class RigidbodySettings
    {
        private Rigidbody _rigidbody;
        public Rigidbody rigidbody {get { return _rigidbody; }}

        private bool _isKinematic;        
        public bool isKinematic { get { return _isKinematic; } }

        private Vector3 _velocity;
        public Vector3 velocity { get { return _velocity; } }

        private Vector3 _angularVelocity;
        public Vector3 angularVelocity { get { return _angularVelocity; } }

        private float _mass;
        public float mass { get { return _mass; } }

        private float _drag;
        public float drag { get { return _drag; } }

        private float _angularDrag;
        public float angularDrag { get { return _angularDrag; } }

        private bool _useGravity;
        public bool useGravity { get { return _useGravity; } }

        private RigidbodyInterpolation _interpolate;
        public RigidbodyInterpolation interpolate { get { return _interpolate; } }

        private CollisionDetectionMode _collisionDetection;
        public CollisionDetectionMode collisionDetection { get { return _collisionDetection; } }

        private RigidbodyConstraints _constraints;
        public RigidbodyConstraints constraints { get { return _constraints; } }

        public delegate void StoreDelegate();
        public StoreDelegate StoreCallback;

        public delegate void RestoreDelegate();
        public RestoreDelegate RestoreCallback;


        public RigidbodySettings(Rigidbody rigidbody)
        {
            _rigidbody = rigidbody;
            _isKinematic = rigidbody.isKinematic;
            _velocity = rigidbody.velocity;
            _angularVelocity = rigidbody.angularVelocity;
            _mass = rigidbody.mass;
            _drag = rigidbody.drag;
            _angularDrag = rigidbody.angularDrag;
            _useGravity = rigidbody.useGravity;
            _interpolate = rigidbody.interpolation;
            _collisionDetection = rigidbody.collisionDetectionMode;
            _constraints = rigidbody.constraints;
        }

        public static void Store(Rigidbody rigidbody)
        {
            RigidbodySettingsHolder settingsHolder = rigidbody.GetComponent<RigidbodySettingsHolder>();
            if (settingsHolder == null)
            {
                settingsHolder = rigidbody.gameObject.AddComponent<RigidbodySettingsHolder>();
            }
            settingsHolder.Set(rigidbody);

            if (settingsHolder.settings.StoreCallback != null)
            {
                settingsHolder.settings.StoreCallback();
                settingsHolder.settings.StoreCallback = null;
            }
        }

        public static void Restore(Rigidbody rigidbody)
        {
            RigidbodySettingsHolder settingsHolder = rigidbody.GetComponent<RigidbodySettingsHolder>();
            if (settingsHolder != null)
            {
                rigidbody.isKinematic = settingsHolder.settings.isKinematic;
                rigidbody.velocity = settingsHolder.settings.velocity;
                rigidbody.angularVelocity = settingsHolder.settings.angularVelocity;
                rigidbody.mass = settingsHolder.settings.mass;
                rigidbody.drag = settingsHolder.settings.drag;
                rigidbody.angularDrag = settingsHolder.settings.angularDrag;
                rigidbody.useGravity = settingsHolder.settings.useGravity;
                rigidbody.interpolation = settingsHolder.settings.interpolate;
                rigidbody.collisionDetectionMode = settingsHolder.settings.collisionDetection;
                rigidbody.constraints = settingsHolder.settings.constraints;
                Object.Destroy(settingsHolder);

                if (settingsHolder.settings.RestoreCallback != null)
                {
                    settingsHolder.settings.RestoreCallback();
                    settingsHolder.settings.RestoreCallback = null;
                }
            }
            else
                Debug.LogError("Cannot restore rigidbody (" + rigidbody.name + ")! No settings holder found");
        }


    }

    /// <summary>
    /// Reflection-style struct to temporarily store the values of a Rigidbody2D
    /// so that they can be restored after pausing.
    /// </summary>
    public class Rigidbody2DSettings
    {
        private Rigidbody2D _rigidbody;
        public Rigidbody2D rigidbody { get { return _rigidbody; } }

        private bool _isKinematic;
        public bool isKinematic { get { return _isKinematic; } }

        private Vector2 _velocity;
        public Vector2 velocity { get { return _velocity; } }

        private float _angularVelocity;
        public float angularVelocity { get { return _angularVelocity; } }

        private float _mass;
        public float mass { get { return _mass; } }

        private float _linearDrag;
        public float linearDrag { get { return _linearDrag; } }

        private float _angularDrag;
        public float angularDrag { get { return _angularDrag; } }

        private float _gravityScale;
        public float gravityScale { get { return _gravityScale; } }

        private RigidbodyInterpolation2D _interpolate;
        public RigidbodyInterpolation2D interpolate { get { return _interpolate; } }

        private CollisionDetectionMode2D _collisionDetection;
        public CollisionDetectionMode2D collisionDetection { get { return _collisionDetection; } }

        private RigidbodySleepMode2D _sleepingMode;
        public RigidbodySleepMode2D sleepMode { get { return _sleepingMode; } }

        private RigidbodyConstraints2D _constraints;
        public RigidbodyConstraints2D constraints { get { return _constraints; } }
        public Rigidbody2DSettings(Rigidbody2D rigidbody)
        {
            _rigidbody = rigidbody;
            _isKinematic = rigidbody.isKinematic;
            _velocity = rigidbody.velocity;
            _angularVelocity = rigidbody.angularVelocity;
            _mass = rigidbody.mass;
            _linearDrag = rigidbody.drag;
            _angularDrag = rigidbody.angularDrag;
            _gravityScale = rigidbody.gravityScale;
            _interpolate = rigidbody.interpolation;
            _collisionDetection = rigidbody.collisionDetectionMode;
            _sleepingMode = rigidbody.sleepMode;
            _constraints = rigidbody.constraints;
        }

        public static void Store(Rigidbody2D rigidbody)
        {
            Rigidbody2DSettingsHolder settingsHolder = rigidbody.GetComponent<Rigidbody2DSettingsHolder>();
            if (settingsHolder == null)
            {
                settingsHolder = rigidbody.gameObject.AddComponent<Rigidbody2DSettingsHolder>();
            }
            settingsHolder.Set(rigidbody);
        }

        public static void Restore(Rigidbody2D rigidbody)
        {
            Rigidbody2DSettingsHolder settingsHolder = rigidbody.GetComponent<Rigidbody2DSettingsHolder>();
            if (settingsHolder != null)
            {
                rigidbody.isKinematic = settingsHolder.settings.isKinematic;
                rigidbody.velocity = settingsHolder.settings.velocity;
                rigidbody.angularVelocity = settingsHolder.settings.angularVelocity;
                rigidbody.mass = settingsHolder.settings.mass;
                rigidbody.drag = settingsHolder.settings.linearDrag;
                rigidbody.angularDrag = settingsHolder.settings.angularDrag;
                rigidbody.gravityScale = settingsHolder.settings.gravityScale;
                rigidbody.interpolation = settingsHolder.settings.interpolate;
                rigidbody.collisionDetectionMode = settingsHolder.settings.collisionDetection;
                rigidbody.sleepMode = settingsHolder.settings.sleepMode;
                rigidbody.constraints = settingsHolder.settings.constraints;
                Object.Destroy(settingsHolder);
            }
            else
                Debug.LogError("Cannot restore rigidbody (" + rigidbody.name + ")! No settings holder found");
        }
    }


    /// <summary>
    /// Pauses or resumes rigidbodies
    /// </summary>
    public class RigidbodyPauser
    {
        private static RigidbodySettings[] pausedRigidBodies;
        private static Rigidbody2DSettings[] pausedRigidBodies2D;

        public static void PauseAll()
        {
            Rigidbody[] allRb = GameObject.FindObjectsOfType<Rigidbody>();
            pausedRigidBodies = new RigidbodySettings[allRb.Length];
            for (int i = 0; i < allRb.Length; i++)
            {
                if (allRb != null)
                {
                    pausedRigidBodies[i] = new RigidbodySettings(allRb[i]);
                    Pause(allRb[i]);
                }
            }

            Rigidbody2D[] allRb2D = GameObject.FindObjectsOfType<Rigidbody2D>();
            pausedRigidBodies2D = new Rigidbody2DSettings[allRb2D.Length];
            for (int i = 0; i < allRb2D.Length; i++)
            {
                if (allRb != null)
                {
                    pausedRigidBodies2D[i] = new Rigidbody2DSettings(allRb2D[i]);
                    Pause(allRb2D[i]);
                }
            }
        }

        public static void ResumeAll()
        {
            for (int i = 0; i < pausedRigidBodies.Length; i++)
            {
                if (pausedRigidBodies[i].rigidbody != null)
                {
                    RigidbodySettings.Restore(pausedRigidBodies[i].rigidbody);
                }
            }
            pausedRigidBodies = null;

            for (int i = 0; i < pausedRigidBodies2D.Length; i++)
            {
                if (pausedRigidBodies2D[i].rigidbody != null)
                {
                    Rigidbody2DSettings.Restore(pausedRigidBodies2D[i].rigidbody);
                }
            }
            pausedRigidBodies2D = null;
        }

        public static void Pause(Rigidbody rigidbody)
        {
            RigidbodySettings.Store(rigidbody);
            rigidbody.isKinematic = true;
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
        }

        public static void Pause(Rigidbody2D rigidbody)
        {
            Rigidbody2DSettings.Store(rigidbody);
            rigidbody.isKinematic = true;
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = 0f;
        }
    }


    /// <summary>
    /// Monobehaviour to hold rigidbody settings
    /// </summary>
    public class RigidbodySettingsHolder : MonoBehaviour
    {
        public RigidbodySettings settings;
        public void Set(Rigidbody rigidbody)
        {
            settings = new RigidbodySettings(rigidbody);
        }
    }


    /// <summary>
    /// Monobehaviour to hold rigidbody2d settings
    /// </summary>
    public class Rigidbody2DSettingsHolder : MonoBehaviour
    {
        public Rigidbody2DSettings settings;
        public void Set(Rigidbody2D rigidbody)
        {
            settings = new Rigidbody2DSettings(rigidbody);
        }
    }
}
