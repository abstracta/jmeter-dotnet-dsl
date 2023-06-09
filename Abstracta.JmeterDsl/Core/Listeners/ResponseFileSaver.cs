namespace Abstracta.JmeterDsl.Core.Listeners
{
    /// <summary>
    /// Generates one file for each response of a sample/request.
    /// <br/>
    /// This element is dependant of the scope: this means that if you add it at test plan level it will
    /// generate files for all samplers in test plan, if added at thread group level then it will
    /// generate files for samplers only in the thread group, and if you add it at sampler level it will
    /// generate files only for the associated sampler.
    /// <br/>
    /// By default, it will generate one file for each response using the given (which might include the
    /// directory location) prefix to create the files and adding an incremental number to each response
    /// and an extension according to the response mime type.
    /// </summary>
    public class ResponseFileSaver : BaseListener
    {
        private readonly string _fileNamePrefix;

        public ResponseFileSaver(string fileNamePrefix)
        {
            _fileNamePrefix = fileNamePrefix;
        }
    }
}
