using _365.Core.Properties;
using System;
using ZXing;



namespace _365.Core
{
    internal class ExtractQR
    {
        public QRTotp GetTOTP()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();


            fileDialog.Filter = "Image Files (*.png;*.jpg;*.jpeg;*.gif;*.bmp)|*.png;*.jpg;*.jpeg;*.gif;*.bmp|All files (*.*)|*.*";
            fileDialog.FilterIndex = 1;
            fileDialog.RestoreDirectory = true;
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                // Decode the QR code
                Bitmap image = new Bitmap(fileDialog.FileName);
                BarcodeReader reader = new BarcodeReader { AutoRotate = true, TryHarder = true };

                Result result; 
                try
                {
                    result = reader.Decode(image);
                }
                catch (Exception)
                {
                    return new QRTotp { secret = "error" };
                }

                string decoded = result?.Text?.Trim();
                if(!string.IsNullOrWhiteSpace(decoded))
                    return ExtractQRCodeData(decoded);
            }
            return new QRTotp { secret = "error" };
        }



        static QRTotp ExtractQRCodeData(string qrData)
        {
            QRTotp qRTOTP = new QRTotp();

            string decodedData = Uri.UnescapeDataString(qrData);

            // Extract email
            int emailIndex = decodedData.IndexOf("totp/") + 5;
            int issuerIndex = decodedData.IndexOf("?secret=");
            qRTOTP.mail = issuerIndex != -1 ? decodedData.Substring(emailIndex, issuerIndex - emailIndex).Split(':')[1] : null;

            // Extract secret key
            int secretIndex = decodedData.IndexOf("secret=");
            if (secretIndex != -1)
            {
                int endOfSecretIndex = decodedData.IndexOf('&', secretIndex);
                if (endOfSecretIndex == -1)
                {
                    qRTOTP.secret = decodedData.Substring(secretIndex + 7);
                }
                else
                {
                    // Calculate the length of the secret key
                    int length = endOfSecretIndex - secretIndex - 7;

                    // Extract the secret key based on its length
                    qRTOTP.secret = decodedData.Substring(secretIndex + 7, length);
                }
            }

            return qRTOTP;
        }
    }
}
