using DualPantoFramework;
namespace UnityEngine
{


    public class RailPolyline : ColliderPolyline
    {
        public string[] texts;
        public float displacement = 0.3f;
        public RailPolyline()
        {

            this.gizmoColor = Color.cyan;
        }
        protected override void CreateObstacle(int i)
        {
            Rail r = this.gameObject.AddComponent<Rail>();
            if (i < texts.Length)
            {
                r.text = texts[i];
            }

            r.CreateObstacle(points[i], points[i + 1], displacement);
            r.Enable();

            // create new trigger for sound
            GameObject newObj = new GameObject("RailTrigger");
            newObj.transform.parent = transform;
            newObj.transform.position = new Vector3(
                (points[i].x + points[i + 1].x) / 2,
                0,
                (points[i].y + points[i + 1].y) / 2
            );
            BoxCollider newTrigger = newObj.AddComponent<BoxCollider>();
            newTrigger.isTrigger = true;
            newTrigger.size = new Vector3(
                displacement*2, // <--- width of trigger
                10,
                Vector2.Distance(points[i], points[i + 1])
            );
            newObj.transform.LookAt(new Vector3(points[i].x, 0, points[i].y), Vector3.up);
            newObj.AddComponent<RailTrigger>();
        }

        void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Debug.LogWarning("Player on rail!");
            }
        }
    }
    

    public class RailTrigger : MonoBehaviour
    {
        AudioSource audioSource;

        private void Start()
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = Resources.Load<AudioClip>("railSound");
            audioSource.loop = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                audioSource.Play();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                audioSource.Stop();
            }
        }
    }
}
