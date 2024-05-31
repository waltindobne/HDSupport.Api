using Firebase.Auth;
using Firebase.Storage;
using System.Net.Sockets;

namespace HD_Support_API.Metodos
{
    public class FirebaseMethods
    {
        public static async Task<FirebaseStorage> FirebaseStorageCustom(string ApiKey, string AuthEmail, string AuthPassword, string Bucket)
        {
            var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
            var loginInfo = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, AuthPassword);
            var storage = new FirebaseStorage(Bucket, new FirebaseStorageOptions
            {
                AuthTokenAsyncFactory = () => Task.FromResult(loginInfo.FirebaseToken),
                ThrowOnCancel = true
            });
            return storage;
        }
    }
}
