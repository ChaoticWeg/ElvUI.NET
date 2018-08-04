using System.Threading.Tasks;

namespace ElvUI_Update.Workers
{
    public abstract class Worker<T>
    {
        protected readonly MainForm _form;

        public Worker(MainForm form)
        {
            _form = form;
        }

        public abstract Task<T> Run();
    }
}
