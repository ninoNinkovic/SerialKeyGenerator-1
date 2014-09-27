using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SKG
{
    class SerialKeyGenerator
    {

        // Random class for generating random numbers
        private static Random _Random = new Random();

        // Prefix or name of the application to differentiate the keys and validate if the key belongs to this application
        private string _Prefix { set; get; }

        // Alphabets for placeing in key when date and prefix is out of boundry
        private string _ABC = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        // Limit the days
        private int _LimitDays { get; set; }


        public SerialKeyGenerator()
        {
            this._LimitDays = 30;
            this._Prefix = "ABCD";
        }

        public SerialKeyGenerator(string Prefix, int LimitDays)
        {
            this._LimitDays = LimitDays;
            this._Prefix = Prefix;
        }

        public string Generate()
        {
            // Take the first five letters
            _Prefix = new string(_Prefix.Take(4).ToArray());

            // Get current date and add days for limiting the expiration dates
            string iDate = DateTime.Now.Date.AddDays(_LimitDays).ToString("MMddyyyy");

            /* Random number to place the Prefix inside each key
             * 
             * Example:
             * _Prefix = "ABCDE";
             * 
             * rPrefix = _Random.Next(0, 4); output: 2
             * Key += 2XAXX
             * 
             * rPrefix = _Random.Next(0, 4); output: 0 
             * Key += BXXXX
             * 
             * rPrefix = _Random.Next(0, 4); output: 3
             * Key += 3XXCX
             * 
             * rPrefix = _Random.Next(0, 4); output: 2
             * Key += 2XDXX
             * 
             * rPrefix = _Random.Next(0, 4); output: 1
             * Key += 1EXXX
             * 
             * */
            int rPrefix = 0;

            // Check if Prefix index is inside boundry
            int iPrefix = 0;

            /* Random number for Alphabets
             * Example:
             * _ABC = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
             * 
             * aIndex = _Random.Next(0, 25);
             * Key += _ABC[aIndex].ToString();
             * 
             * 
             * 
             */
            int aIndex = 0;

            /* Check if date index inside boundry
             * 
             */
            int dIndex = 0;

            /* Boolean variable for alternate index
             * if(dateLoop){
             *      Key += iDate[dIndex];
             *      dIndex++;
             *      dateLoop = false;
             * }
             *
             */
            bool dateLoop = true;

            // string variable to store the key
            string Key = "";


            // Loop to create keys XXXXX-XXXXX-XXXXX-XXXXX-XXXXX
            for (int g = 0; g < 4; g++)
            {
                // Random number to place the Prefix inside each key
                rPrefix = _Random.Next(0, 4);

                // Initialize true for each key loop
                dateLoop = true;

                // 5 loops for each key XXXX
                for (int i = 0; i <= 4; i++)
                {
                    // Random number for Alphabets
                    aIndex = _Random.Next(0, 25);

                    /* if current index of key is zero and rPrefix is not zero then add rPrefix to the key.
                     * When validating we will check if first index is number then get the character from 
                     * key using the index if not the first character is itself a character of Prefix
                     * 
                     * Example:
                     * 31LA0 (A)
                     * First character is number and we should take the 3rd index to add in Prefix for verification
                     * or
                     * T2L0N (T)
                     * First character is not a number then its part of the prefix
                     */
                    if (i == 0 && i != rPrefix)
                    {
                        Key += rPrefix.ToString();
                    }
                    else
                    {
                        /* if current index of the key matches with the index of the prefix and prefix index is in boundry then
                         * add the prefix character in the key and increment the prefix index
                         * else add date in the curren key and increment the date index
                         */
                        if (i == rPrefix && iPrefix < _Prefix.Length)
                        {
                            Key += _Prefix[iPrefix];
                            iPrefix++;
                        }
                        else
                        {
                            /* if date index is inside date boundy then dateLoop is true then
                             * add date character
                             */
                            if (dateLoop
                                && dIndex < iDate.Length
                                )
                            {
                                Key += iDate[dIndex];
                                dIndex++;
                                dateLoop = false;
                            }
                            else
                            {
                                /* if date is false then add Alphabets in the key                                 * 
                                 */
                                if (!dateLoop)
                                {
                                    Key += _ABC[aIndex].ToString();
                                    dateLoop = true;
                                }
                                else
                                {

                                    // add date character (Alternate loop)
                                    if (dIndex < iDate.Length)
                                    {
                                        Key += iDate[dIndex];
                                    }
                                    else
                                    {
                                        Key += _ABC[aIndex].ToString();
                                    }
                                    dateLoop = false;
                                }
                            }
                        }
                    }
                }

                // Add seperate after each key
                Key += "-";
            }

            // Take the first 29 characters XXXXX-XXXXX-XXXXX-XXXXX-XXXXX
            Key = new string(Key.Take(23).ToArray());

            return Key;
        }

        public int ValidateKey(string ProductKey)
        {
            try
            {
                string[] _ProductKeys = ProductKey.Split(new char[] { '-' });

                string Prefix = "";
                string Date = "";
                foreach (string _Key in _ProductKeys)
                {

                    for (int i = 0; i < _Key.Length; i++)
                    {

                        string PrefixKey = _Key[i].ToString();

                        if (i == 0)
                        {
                            if (Regex.IsMatch(PrefixKey, @"\d"))
                            {
                                int _PrefixIndex = Convert.ToInt32(PrefixKey);
                                Prefix += _Key[_PrefixIndex].ToString();
                            }
                            else
                            {
                                Prefix += PrefixKey;
                            }
                        }
                        else
                        {
                            if (Regex.IsMatch(PrefixKey, @"\d"))
                            {
                                Date += PrefixKey;
                            }
                        }
                    }
                }

                /*
                 * 0 = Invalid Key
                 * 1 = Expired Key
                 * 2 = Valid   Key
                 */

                if (Prefix != _Prefix)
                    return 0;


                int _Month = Convert.ToInt32(Date.Substring(0, 2));
                int _Day = Convert.ToInt32(Date.Substring(2, 2));
                int _Year = Convert.ToInt32(Date.Substring(4));


                DateTime _Date = new DateTime(_Year, _Month, _Day);

                DateTime cDate = Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yyyy"));

                if (cDate > _Date)
                    return 1;

                return 2;

            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}

// Usage:

SerialKeyGenerator obj = new SerialKeyGenerator();
string ProductKey = obj.Generate();
int Status = obj.ValidateKey(ProductKey);

switch (Status)
{
	case 0:
		richTextBox2.AppendText("Invalid key!");
		break;

	case 1:
		richTextBox2.AppendText("Expired key...");
		break;

	case 2:
		richTextBox2.AppendText("Valid Key!!!");
		break;
}