using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Walletobjects.v1;
using Google.Apis.Walletobjects.v1.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using static MMA.WebApi.Shared.Enums.Declares;

namespace MMA.WebApi.Shared.Helpers
{
    public class GoogleWalletHelper
    {
        /// <summary>
        /// Path to service account key file from Google Cloud Console. Environment
        /// variable: GOOGLE_APPLICATION_CREDENTIALS.
        /// </summary>
        public static string keyFilePath;

        /// <summary>
        /// Service account credentials for Google Wallet APIs
        /// </summary>
        public static ServiceAccountCredential credentials;

        /// <summary>
        /// Google Wallet service client
        /// </summary>
        public static WalletobjectsService service;
        private readonly IConfiguration _configuration;

        public GoogleWalletHelper(string path, IConfiguration configuration)
        {
            keyFilePath = path;

            Auth();
            _configuration = configuration;
        }

        public void Auth()
        {
            credentials = (ServiceAccountCredential)
                GoogleCredential
                    .FromFile(keyFilePath)
                    .CreateScoped(
                        new List<string> { "https://www.googleapis.com/auth/wallet_object.issuer" }
                    )
                    .UnderlyingCredential;

            service = new WalletobjectsService(
                new BaseClientService.Initializer() { HttpClientInitializer = credentials }
            );
        }

        public string CreateJWTNewObjects(
            string firstName,
            string lastName,
            string ECardSequence,
            string issuerId,
            string classSuffix,
            string objectSuffix
        )
        {
            var baseUrl = _configuration["BaseURL:Url"];

            JsonSerializerSettings excludeNulls = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            GenericObject newObject = new GenericObject
            {
                Id = $"{issuerId}.{objectSuffix}",
                ClassId = $"{issuerId}.{classSuffix}",
                GenericType = "GENERIC_TYPE_UNSPECIFIED",
                Barcode = new Barcode { Type = "QR_CODE", Value = "https://mazayaoffers.ae" },
                CardTitle = new LocalizedString
                {
                    DefaultValue = new TranslatedString
                    {
                        Language = "en-US",
                        Value = "Mazaya E Card"
                    }
                },
                Header = new LocalizedString
                {
                    DefaultValue = new TranslatedString
                    {
                        Language = "en-US",
                        Value = $"{firstName} {lastName}"
                    }
                },
                Subheader = new LocalizedString
                {
                    DefaultValue = new TranslatedString
                    {
                        Language = "en-US",
                        Value = "Offers And Discounts"
                    }
                },
                TextModulesData = new List<TextModuleData>
                {
                    new TextModuleData
                    {
                        Id = "points",
                        Header = "Card No.",
                        Body = $"{ECardSequence}"
                    },
                    new TextModuleData
                    {
                        Id = "_for_any_enquiries,_please_contact_027071717",
                        Header = "02701717 \u200Eللاستفسارات، يرجى الاتصال على الرقم التالي",
                        Body = "For any enquiries, please contact 027071717"
                    }
                },
                HexBackgroundColor = "#0A2F6A",
                Logo = new Image
                {
                    SourceUri = new ImageUri
                    {
                        Uri = baseUrl + "assets/images/walletsImage/logo.png"
                    },
                    ContentDescription = new LocalizedString
                    {
                        DefaultValue = new TranslatedString
                        {
                            Language = "en-US",
                            Value = "Generic card logo"
                        }
                    },
                }
            };

            GenericClass newClass = new GenericClass
            {
                Id = $"{issuerId}.{classSuffix}",
                ClassTemplateInfo = new ClassTemplateInfo
                {
                    CardTemplateOverride = new CardTemplateOverride
                    {
                        CardRowTemplateInfos = new List<CardRowTemplateInfo>
                        {
                            new CardRowTemplateInfo
                            {
                                OneItem = new CardRowOneItem
                                {
                                    Item = new TemplateItem
                                    {
                                        FirstValue = new FieldSelector
                                        {
                                            Fields = new List<FieldReference>
                                            {
                                                new FieldReference
                                                {
                                                    FieldPath = "object.textModulesData[0]"
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            new CardRowTemplateInfo
                            {
                                OneItem = new CardRowOneItem
                                {
                                    Item = new TemplateItem
                                    {
                                        FirstValue = new FieldSelector
                                        {
                                            Fields = new List<FieldReference>
                                            {
                                                new FieldReference
                                                {
                                                    FieldPath = "object.textModulesData[1]"
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            // Create JSON representations of the class and object
            JObject serializedClass = JObject.Parse(
                JsonConvert.SerializeObject(newClass, excludeNulls)
            );
            JObject serializedObject = JObject.Parse(
                JsonConvert.SerializeObject(newObject, excludeNulls)
            );

            // Create the JWT as a JSON object
            JObject jwtPayload = JObject.Parse(
                JsonConvert.SerializeObject(
                    new
                    {
                        iss = credentials.Id,
                        aud = "google",
                        origins = new List<string> { "https://mazayaoffers.ae/" },
                        typ = "savetowallet",
                        payload = JObject.Parse(
                            JsonConvert.SerializeObject(
                                new
                                {
                                    // The listed classes and objects will be created
                                    // when the user saves the pass to their wallet
                                    genericObjects = new List<JObject> { serializedObject },
                                    genericClasses = new List<JObject> { serializedClass }
                                }
                            )
                        )
                    }
                )
            );

            // Deserialize into a JwtPayload
            JwtPayload claims = JwtPayload.Deserialize(jwtPayload.ToString());

            // The service account credentials are used to sign the JWT
            RsaSecurityKey key = new RsaSecurityKey(credentials.Key);
            SigningCredentials signingCredentials = new SigningCredentials(
                key,
                SecurityAlgorithms.RsaSha256
            );
            JwtSecurityToken jwt = new JwtSecurityToken(new JwtHeader(signingCredentials), claims);
            string token = new JwtSecurityTokenHandler().WriteToken(jwt);

            return token;
        }

        public string CreateJWTMembershipObjects(
            string firstName,
            string lastName,
            string azureStoragePath,
            string expiredDate,
            string ECardSequence,
            string photoUrl,
            WalletCardType cardType,
            string issuerId,
            string classSuffix,
            string objectSuffix
        )
        {
            var baseUrl = _configuration["BaseURL:Url"];

            var logoUri = baseUrl + "assets/images/walletsImage/logo.png"; //logo
            var backgroundColor = "#0A2F6A";
            var headerContent = "Offers And Discounts";
            if (cardType == WalletCardType.FamilyEntertainment)
            {
                logoUri = baseUrl + "assets/images/walletsImage/logoGold.png"; //GOLDEN
                backgroundColor = "#E6AB2F";
                headerContent = "Family Entertainment";
            }
            else if (cardType == WalletCardType.LeisureAndFamilyEntertainment)
            {
                logoUri = baseUrl + "assets/images/walletsImage/logoGray.png"; //GRAY
                backgroundColor = "#9A9A9C";
                headerContent = "Leisure And Family Entertainment";
            }
            else if (cardType == WalletCardType.HealtAndLeisure)
            {
                logoUri = baseUrl + "assets/images/walletsImage/logoBlue.png"; //BLUE
                backgroundColor = "#2FA0D7";
                headerContent = "Health And Leisure";
            }
            // Ignore null values when serializing to/from JSON
            JsonSerializerSettings excludeNulls = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            // See link below for more information on required properties
            // https://developers.google.com/wallet/generic/rest/v1/genericclass
            GenericClass newClass = new GenericClass
            {
                Id = $"{issuerId}.{classSuffix}",
                ClassTemplateInfo = new ClassTemplateInfo
                {
                    CardTemplateOverride = new CardTemplateOverride
                    {
                        CardRowTemplateInfos = new List<CardRowTemplateInfo>
                        {
                            new CardRowTemplateInfo
                            {
                                ThreeItems = new CardRowThreeItems
                                {
                                    StartItem = new TemplateItem
                                    {
                                        FirstValue = new FieldSelector
                                        {
                                            Fields = new List<FieldReference>
                                            {
                                                new FieldReference
                                                {
                                                    FieldPath = "object.textModulesData[0]"
                                                }
                                            }
                                        }
                                    },
                                    MiddleItem = new TemplateItem
                                    {
                                        FirstValue = new FieldSelector
                                        {
                                            Fields = new List<FieldReference>
                                            {
                                                new FieldReference
                                                {
                                                    FieldPath = "object.textModulesData[2]"
                                                }
                                            }
                                        }
                                    },
                                    EndItem = new TemplateItem
                                    {
                                        FirstValue = new FieldSelector
                                        {
                                            Fields = new List<FieldReference>
                                            {
                                                new FieldReference
                                                {
                                                    FieldPath = "object.imageModulesData[0]"
                                                }
                                            }
                                        }
                                    }
                                },
                            },
                            new CardRowTemplateInfo
                            {
                                OneItem = new CardRowOneItem
                                {
                                    Item = new TemplateItem
                                    {
                                        FirstValue = new FieldSelector
                                        {
                                            Fields = new List<FieldReference>
                                            {
                                                new FieldReference
                                                {
                                                    FieldPath = "object.textModulesData[3]"
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            new CardRowTemplateInfo
                            {
                                OneItem = new CardRowOneItem
                                {
                                    Item = new TemplateItem
                                    {
                                        FirstValue = new FieldSelector
                                        {
                                            Fields = new List<FieldReference>
                                            {
                                                new FieldReference
                                                {
                                                    FieldPath = "object.textModulesData[1]"
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            // See link below for more information on required properties
            // https://developers.google.com/wallet/generic/rest/v1/genericobject
            GenericObject newObject = new GenericObject
            {
                Id = $"{issuerId}.{objectSuffix}",
                ClassId = $"{issuerId}.{classSuffix}",
                State = "ACTIVE",
                Header = new LocalizedString
                {
                    DefaultValue = new TranslatedString
                    {
                        Language = "en-US",
                        Value = $"{firstName} {lastName}"
                    }
                },
                Subheader = new LocalizedString
                {
                    DefaultValue = new TranslatedString
                    {
                        Language = "en-US",
                        Value = $"{headerContent}"
                    }
                },
                TextModulesData = new List<TextModuleData>
                {
                    new TextModuleData
                    {
                        Id = "points",
                        Header = "Card No.",
                        Body = ECardSequence
                    },
                    new TextModuleData
                    {
                        Id = "_for_any_enquiries,_please_contact_027071717",
                        Header = "02701717 \u200Eللاستفسارات، يرجى الاتصال على الرقم التالي",
                        Body = "For any enquiries, please contact 027071717"
                    },
                    new TextModuleData
                    {
                        Header = "",
                        Body = "",
                        Id = "Empty"
                    },
                    new TextModuleData
                    {
                        Header = "Expiry Date",
                        Body = expiredDate,
                        Id = "ExpiryDate"
                    }
                },
                ImageModulesData = new List<ImageModuleData>
                {
                    new ImageModuleData
                    {
                        MainImage = new Image
                        {
                            SourceUri = new ImageUri
                            {
                                Uri =
                                    $"https://{azureStoragePath}.blob.core.windows.net/documents/{photoUrl}"
                            },
                            ContentDescription = new LocalizedString
                            {
                                DefaultValue = new TranslatedString
                                {
                                    Language = "en-US",
                                    Value = "Image module description"
                                }
                            }
                        },
                        Id = "IMAGE_MODULE_ID"
                    }
                },
                Barcode = new Barcode { Type = "QR_CODE", Value = "https://mazayaoffers.ae" },
                CardTitle = new LocalizedString
                {
                    DefaultValue = new TranslatedString
                    {
                        Language = "en-US",
                        Value = "Mazaya ++ E Card"
                    }
                },
                HexBackgroundColor = backgroundColor,
                Logo = new Image
                {
                    SourceUri = new ImageUri { Uri = logoUri },
                    ContentDescription = new LocalizedString
                    {
                        DefaultValue = new TranslatedString
                        {
                            Language = "en-US",
                            Value = "Generic card logo"
                        }
                    },
                }
            };

            // Create JSON representations of the class and object
            JObject serializedClass = JObject.Parse(
                JsonConvert.SerializeObject(newClass, excludeNulls)
            );
            JObject serializedObject = JObject.Parse(
                JsonConvert.SerializeObject(newObject, excludeNulls)
            );

            // Create the JWT as a JSON object
            JObject jwtPayload = JObject.Parse(
                JsonConvert.SerializeObject(
                    new
                    {
                        iss = credentials.Id,
                        aud = "google",
                        origins = new List<string> { "https://mazayaoffers.ae/" },
                        typ = "savetowallet",
                        payload = JObject.Parse(
                            JsonConvert.SerializeObject(
                                new
                                {
                                    // The listed classes and objects will be created
                                    // when the user saves the pass to their wallet
                                    genericClasses = new List<JObject> { serializedClass },
                                    genericObjects = new List<JObject> { serializedObject }
                                }
                            )
                        )
                    }
                )
            );

            // Deserialize into a JwtPayload
            JwtPayload claims = JwtPayload.Deserialize(jwtPayload.ToString());

            // The service account credentials are used to sign the JWT
            RsaSecurityKey key = new RsaSecurityKey(credentials.Key);
            SigningCredentials signingCredentials = new SigningCredentials(
                key,
                SecurityAlgorithms.RsaSha256
            );
            JwtSecurityToken jwt = new JwtSecurityToken(new JwtHeader(signingCredentials), claims);
            string token = new JwtSecurityTokenHandler().WriteToken(jwt);

            return token;
        }
    }
}
