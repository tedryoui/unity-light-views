using UnityEngine;

namespace Assets
{

    public abstract class View : MonoBehaviour { }

    public abstract class View<T1> : View
        where T1 : ViewModel, new()
    {
        [SerializeField] private bool _isVisibleByDefault;
        private bool _isVisible = true;

        private T1 _viewModel;
        public T1 ViewModel => _viewModel ??= new T1();

        public bool IsOpened => gameObject.activeInHierarchy;

        protected virtual void Start()
        {
            ViewModel.Initialize();

            if (_isVisible)
            {
                if (_isVisibleByDefault) Open();
                if (!_isVisibleByDefault) Close();
            }
        }
        public virtual void Open()
        {
            gameObject.SetActive(true);
            _isVisible = false;
        }

        public virtual void Close()
        {
            gameObject.SetActive(false);
            _isVisible = false;
        }
    }

    public abstract class ViewModel
    {
        public virtual void Initialize() { }
    }

    public abstract class ViewModel<T1> : ViewModel
        where T1 : View
    {
        private T1 _view;
        public T1 View => _view;

        public void BindView(T1 view) => _view = view;
    }
}