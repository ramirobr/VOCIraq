Use [IQVoc]

declare @image VARBINARY(max)

set @image = 0xFFD8FFE000104A46494600010101006000600000FFE1005A4578696600004D4D002A00000008000503010005000000010000004A030300010000000100000000511000010000000101000000511100040000000100000EC3511200040000000100000EC300000000000186A00000B18FFFDB0043000201010201010202020202020202030503030303030604040305070607070706070708090B0908080A0807070A0D0A0A0B0C0C0C0C07090E0F0D0C0E0B0C0C0CFFDB004301020202030303060303060C0807080C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0CFFC00011080060014003012200021101031101FFC4001F0000010501010101010100000000000000000102030405060708090A0BFFC400B5100002010303020403050504040000017D01020300041105122131410613516107227114328191A1082342B1C11552D1F02433627282090A161718191A25262728292A3435363738393A434445464748494A535455565758595A636465666768696A737475767778797A838485868788898A92939495969798999AA2A3A4A5A6A7A8A9AAB2B3B4B5B6B7B8B9BAC2C3C4C5C6C7C8C9CAD2D3D4D5D6D7D8D9DAE1E2E3E4E5E6E7E8E9EAF1F2F3F4F5F6F7F8F9FAFFC4001F0100030101010101010101010000000000000102030405060708090A0BFFC400B51100020102040403040705040400010277000102031104052131061241510761711322328108144291A1B1C109233352F0156272D10A162434E125F11718191A262728292A35363738393A434445464748494A535455565758595A636465666768696A737475767778797A82838485868788898A92939495969798999AA2A3A4A5A6A7A8A9AAB2B3B4B5B6B7B8B9BAC2C3C4C5C6C7C8C9CAD2D3D4D5D6D7D8D9DAE2E3E4E5E6E7E8E9EAF2F3F4F5F6F7F8F9FAFFDA000C03010002110311003F00FDFCA2B0FC79E3ED0FE19785EE75BF106A969A3E95603335CDCC9E54517D4D780E97FF000562F835AEF8BA1D1935CD422FB5CBE5477B2D8CB15A7FDFDA00FA6A8AE7E6F891A241E13975B6D634B1A4423F79786E97ECD1FF00DB4E95E70DFB68F832EBE30CDE06B3B9D42FB5CB38A59AF3CAB6223B4317FCB226423F7BED401ECF457935FF00ED83E03B1F85161E368B5E17BE1FD4AE62B28AE2DE3DDE5492F118933FEABFEDAE3A8AF39D7BFE0AA9F0BFC3DA469571751F8A8CBAB43E6C56F169864961F69403FBB93D8D007D3F457850FDBF7C072FC25BFF001A4326AD2E83A75EFF0067F9A6C8C7F6B97CAF37F77EBC570B2FFC15F3E1781103A5F8F4E638E6FF00917E5EE6803EAFA2BCC344FDA87C29A9FC15D3BC7D3DFC9A6F87B511FB893508FC99A4FDE98FCBF2F39F332318FF00EBD79B4DFF000553F8426FAD2DD355D5255BB9248A590589F2AD0FFD35F4A00FA628AF1BF8EDFB677847E067C2ED23C5B7326A3AEE93ADC9E559BE8B1C777E7398CC83BF702BAEF0C7C5CD37C51F0B878B2D05D0D316CA4BB922923C4B17943F791C833C49C118F63401DB515E5BFB2F7ED51E1EFDAC7C052F883C3906AB6B6305C1B5906A16DE4CBE60F6AE7F4DFDBBBC0B7FFB417FC2B576D5ACF5D6B996CA2B8BAB5F2AC2EA68FF00E59452FF00CB43401EE545475E59F163F6ABF0F7C18F8A3E1BF09EA967AECBA8789658A1B692CAC4CB6911966F2A3F365FF967CD007ABD15E39AA7ED8FE14F0F7C69D3BC0B7E354B6D4352B97B0B6BC96DBCBB49AE47FCB207D6ACFED13FB57F867F660BEF0EC5E273790C3E25965863B98E34F2A131C793E61CFF002FFEB5007AD515E13FB477EDF7E00FD98754B5D3FC43717F36B1796DF6A8EC6CED7CE9403FEAFCDFF9E79F7ADFF80DFB56786FF687D2E5BCF0FC7AB0B086410FDA6F2DBC9825971FEAA2909FDE49ED401EA7457CD5F12FFE0A99F0D3E11FC42D77C33ADDB78AA3D5740B9FB2CC62D344B14D27FD32FDE735DFFC0CFDAC7C27FB406970EA1A33EA96DA7DFCDF66B1B8D4ACBECB15FCA3EFC7113FEB0D007AB515F2CF8A7FE0AD3F0D3C1DABEAB677BA5F8C4CBA25F4BA7DCF956314B8962FFB6B5A7F037FE0A7DF0E3F686F8AF61E0EF0F5B7899355D47CD16EF7563E4C27CA8BCD3FF2D3D2803E97A2BE6AFDA0FF00E0A87F0C3F674F185DE81A85C6ABAC6B7A79F2AEADF4DB6330B493FE794927635D07C0EFDBFBE1FF00C7CD3AF8695797761AA69D6DF6B974AD422F2750962F27CDF3228BFE5A71E9401EEB457C8307FC169BE0EDC7FAA8BC6327D34B1FFC76BB2F857FF0536F853F1AB53D42CF43BFD6A6BAD2B4DB9D5AEA3974C962315B43FEB0D007D194579B7C07FDA67C2FFB45FC379BC57E1E9F508B45B696585E5BDB536A7319E7FD676F7AF30D73FE0ABFF06341F104BA7BF88B50BAF224F2A4B8B6D2A6920CFF00D74E9401F4C515E716FF00B4DF81B51F853A978DB4EF11D8EB3E1FD26D4DDDC5C69F9B99238BD4C63F794DF803FB4D783BF699D0AFF51F066AADAA5B69F29B5BADF1490CB0CBE9E5C82803D268AF2BF05FED7BE08F885F1AF56F879A66A37571E27D17CC17B07D8E58A284C67127EF08C7535DDF8C3C61A77807C357FAC6B1790D8697A745E75CDC4BC45147401B3457CCF6FF00F055FF0083171AD4B672F88EFECDA293CA33DC691731C19FA915F42F873C4DA7F8C743B2D534BBB86FF4FBF8BCEB7B889F31CC87B8F5A00D3A2B3358D761F0EE93757D72E21B5B48A49A593B471C7D6BCE7F67BFDB17C07FB4CDEDD5B784754BABF9AD2DFED92F99632DA811E7CBCFEF00EF401EB345145007C61FF0575D3AEBC6307C26F08457734365E2AF140B5BAF2FAE33181FFA30D7D01E29FD963C0BE33F8487C1177A0582E80B1F97145145E5CB6DC7FAD8E4EA25FF00A682B87FF82807ECDBAF7ED11F0D34EB8F085E4565E2DF0C5DFF006869BE77FAA97B491FB4871D6BC2BC63F1A7F6A6F1D7C2697C0BFF000AA6FAD7C41751CB637DE2113C621962C1198FFE59472F3FEB0F1C0EF401DA7C66F86DA1FEC75E1EF84DE08F0E6B773E1CF0CEAFE33FB55D5D6A0FF6F264F28E21FDEFFCF594FF003AFABA1F07E996ED2BC7A7D8096597CE90FD98732FFCF4AF9CBC0FFB0CDF58FECEBE04D13C49AA4DAFF8A3C25AE5B6BF2CD717D2CD179A0E25862924E7CAF2F3F91ABFFB4EFED55F10BE176A23C3BE07F873E22F1BEAF347293A8C763247676031FBA1CFFAD93BF140193FB1E685A07C33FDA37E28783F4FF1641E229E5BEFED0B9B6B9F2BED5E6FFCB597E9179B14558BE21D735DD4BFE0AEDA06957B1DDCBE1CB3D1E5BAD30CB1FEE6297ECBFBCF2EB57FE09DFF00B1FEBBF05F53F11F8EBC64B35BF8D3C651C42E609648A59A1FDE79B299258F83E6CBCFE151C9F0D3C50DFF0005658BC46BA2EAE7C231F86BCB7D4BC9FF0044FB4F95E5E3CCFF009E98E280357FE0A83E17D27C59F002C74FD5A4962336A7E5D8F976D14B1FDA7ECD3633E67B7995CF781FF6D8F8AF2685A5DBE97FB3878B6EA28AC6DA3FB4DD6A115AF9BFBAFF00AE55BBFF00055DF877ADFC53FD9D2C34BD03C3FAAF88EF23D6A29A4B7D3A3F3668A2F2A5FDEE3DBFAD65E95FB677C5FD27C196318FD9C3C652DD456F14647DBA1F41E67F2A00E27F6D5F07EB7FB54FC4EFD9FF00C35ABE8FA9683A56B6D2EA1ABE9C63CFF67FFAAFDD49FF004D7CAF347E15F50FC5AFD97FC0DF14FE11DCF83755D12C6D741109862FB3451C32E9FDFCD8A4FF009666BC73F68AF05FC58FDA77F674D03C41E18B4D53E1F78B6D6E25BCB8F0A5C5F01FDA3109BF7714B27FAAF3079625FC6BCBBE227C58FDA73F680F004DE00BCF8432E8E7588BEC5A8EAB1C8238A58B1C91E67EEE3CFE3401E9DFB47F87BC11FB21FECF1E14D3A6D35F5AF0EC3E208BCA9F50BDF34E9B27FADFB5997CBFF965E55705F0D7F6847F803FB447C40F869E22D42C6CFC2DE20825BAD12E0D97D96086EA4B6F37FEFDC918FCC76AADFB5A7EC7FE33F03FEC27E08F0269169AAF8EF5DB0D6C5DEA3F63F3653FEAA6FF0055FF003CA2E457A47EDEFF00B27CDF167E11F87FC47E1FB5BC87C59E074B59628E08C4B7777147D221FF004D223FBC1EE0D00705FF0004DDF8B23E16FEC51F12BC51AADE1BA8FC33A95D4B14D711793E688AD61F2FF775E09E2AF84DACFF00C33BE9FF001AEDBC56759F11DAF8A7ED76D2DB7496597F7B2F95E57FD3D7EEABD17C1DE01F8A9E08FD9AAEB4893E1A7887C5BAC78C7C792EA1A95BEAB6DE579D6B14517952DD7FD7597FF4557B0DFF00FC1233C056DE1E9EF748BEF1569DE2682D6496D65B7D4CFD960BAC64F97176FDE5007D2BF05BE25D9FC5DF85BE1DF14590220F1059457639FF005798F35F39FED65E0E6D6FFE0A0DF052E20D6349B5921596696CAF46018E3941F387FD3539F2E2F72697FE094363E3AF037C2CD77C29E33F0C6BFA0C3A5DF0BBD2E5D46DC43E709799631F49327FEDA556FDB1FE167887C57FF0503F819AE69FE1CD5F55D0B4893FD3EFADE1FF0045D3BF7BFF002D64FE9401E3BFB7CFF6C7ED09F1D7C77E16D2FC29A4B6BDE02862D56C75FB6F361D427B58A28A596D7FE9AFFADFFC855E9BE12D5F4AFDAE7F678F056ABF10ADBC5167AD786AC25FED2B78D25897518FCA8CFDAFCCF2FF007A658A38E58BFEDA7A71E8BF0B7C45E2CB6FDB73C73E1F97C31ACDB7836E631A81D5EF52496D6F25F2A211C56DDA3FF96BFF007EABE72FDAABE08FC55F823F12AE7C37E04D1FC4DAF7C3BF10DCC57F6F1D8C52DDC3A447E6FEF6C3CAFF0055145401DAFED6FF00053E217C38FDAB63F8B7E10F0869FE39D33ECD15CFD8E5B0FB55DC534717922219FDEC71795FBDFF00AEB5EA7E22D7FC1BF1A2C7C2F79AFA6B1A52E85AADAEA92268FE6451D85FC83F75F6AFDD09633FF2CFB75ED5E57FB4868DF187F668FDAF24F88BE0CD275BF1CF84359B6F2A4D15259658ED31E4F9B1F95FF2CBFD5F99163E95ABF0C7E33FC5CFDABFE344F1DC7C3CD4FC01E0D3A6CB617D737D6C49F34FFD75F2BCD1401C259FC69F06FECA1FF050BF8B1E20F1D5DC935AEB844365F67B2375E4C9FBA94F99FF003CFF00D5559D1FE2CDCFEDAFFB577836EFE1945E4F80BC25AE58DFEA56D7091DB4DE64465964BB8A3EB1478C45FF004D6BA9FD97FE19DE4FFF00052DF8C9AA6A9E15D44F87268A4FB2DD6A3A666D2797CD8B98A493FEDAD7DA3A4E8161A15B18ECED6D6CA23DA08845FCA803F36BE10F8F759FD9BBF68FF8B5E23D3BE1DCDF1174FF0010EB975691DB69D2996EF4EF2AEBFD6CB188E5F2A2FDED7D09F017F695F177C50F186A16F77F03EE3E1EC369A75C4DFDB37A044219047FBB1FEABBD7CF7F087E2C78F3F65AFDA9BE2FEB7A7FC28F16F8B21F13EA773E5C96D6B24509FF004B97CA97CD317EF735F40FC25FDBB7C69F197E26699E14D5FE0978B7C39A7EB724B6B75A85E79821B58FCAFF00AE5401E69FF0482F81FA078FFC17E2CF881E20B0D3B5DF116A3AC4B6864BD8BED66D7FE5ACBFEB7FE7AF9B5EE63FE09EDE0187F69AB5F893A73DD68FAA59B79C74CB2F2E1B299C8F2E43247DFCD18AF967C23A6FC63FF8261FC4DF11E99A1783351F1B781755B9325AB431492C7267FD57FABFF552F48FF2AEABF65BF821F183F681FDB121F8BDE39D2B52F04E9BA74A248AC9E492D4DDC71C788ADE38B8CC59393E6FBD005AFF0082D0F83B44F0AFC31F01FD8347D3AC259B57962FF42B68A2F363F2BA57D27F113E14F853C2FF00B3DF8A6EECF47F0F78726B9F0CDD4536A11D8450F9311B5FDE194C7F8D782FFC16A749D435BF87FF000EA3B2B2BBBA316B12F9BF66B6F3BCAFDCD7D29FB41E3FE1933C651CD1DD4C0F85AEB31C51E65FF8F53FAE6803F3E3FE165DE7C39FF82564B068FA8C52C7AB78F25D3A2923F362866B5FF5BE545FF2D7CAAFB0FE187FC13DBE1BDE7C26B51E2BD12CFC5BE24D72D63BCD5F5ABD526F2EEE6488665F33AC7F857CDFF02FE0E788BE2BFF00C1329E1F0BE9B24DE21D03C41737F6D677965F35D47E518A48A3F493F79D73D62AD1F01FFC165F54F08F81A4D23C51E03961F16E9D198A3F2E4FB2DAF4FDD7991CBFBD1401E6DF0CFC37A87ECF9F16FF0068AF85965733CBA28F0B6A7345E64BD7CA1E6C52FF00D75F2A5F2AB8DFD853E336B1FB22FC59F0B78A7558A5B5F04F8EFCDB0BD97FE58CB1C52F952CDFF6CA5AF4FF00D9EBE1178BBC63F0C7E39FC65F1A69F35AA789BC337F1D8FDA21F2A5BB925FDECB2C71F68BF77145EF5D8FC11FD9663FDA83FE0947A4E9766B00F11693797FA8690FDCCBE74BFBBFFB6BFD680357F652D52DA5FF0082BD7C567B77130BAB4BAFDE274FF5B6B5ED9FF053BF865E21F8B5FB226B3A7F86ECEEB50BFB6BAB7BC92CAD8FEF6F228893245F957C75FF000474BF926FDAF6EEDEF722EAD343BEFF0059FEBBCDF3A2F37CDAFB4FFE0A1F0FC438BF677BAD4BE1AEA7AA58EBBA55CC575731E9DCCD776C3FD688FDF1FD6803E118FE29FC279BE07E99E1CF885F0CF58F0EF89F50B53683C536623BAD44CB10F2BCDFB3CBE54A39FF009658AFBBBFE09E1E18F0FF0084BF66ED36CBC2BE36B8F1D6822EA596DAFA588452DA893FE5D7CA1FEA845E95F1DF8FFF00E0A63E0FF8B7FB3EDD687E30F86B6BACFC4096CBEC125E49145E4F99E4FF00C7D79BFEB62F5ADCFD91FE286B7FF04FBFD88B53F1C7883C33A95F45E2AF114515969AF20B598C5E4FFC7C9CFF00D73A00FA53FE0A81F1E21F831FB30EA56EB398AEFC5927F6246D1FFAD8A39636F364FC23CFE75F1E7ECDD61A9FEC19FB5FFC38B8D56E258F40F891A445FF006C63BAFF009652FF00D7297CAFFBFB5D2FED9BE2CBCFDBBFF6B2F86BF0FF00C3D2DB5B795A445A85CC575FBE86CE5BA845CCBE60FF00969E54422AC2FDB7FF0060EF1FFC1EF8510F8CBC43F1126F195B787A48ED628A58A5F3B4E8A497FE5979B2FBD007EA643D29F5F14FC1EFF82A3583699F077C397BA4CFA96B1E37B386D6F35117F1470DA5C89BECA7A8FDE13E59938ED5F6B500474549450047454945004745494500474549450014514500474515F247867C30DE3EFF00828DFC4AF0DEA3ABF8964D0ACFC3F63AADADB5B6B1756B15ADCCA62F3258BCB90726803EB7A2BE60FD88BE2B789EFF00E3DFC58F877AE788EE7C57A5F81EFA23A66A17433742397FE594B27490FBD687EDC5F1EFC47E0EF10F80FE1D7822EC695E28F88DA9FD93FB47C9F3069D6A3FD6CA3FE9A5007D1F4578278EBF64336BF0FEE5FC1DE21F1468DE34D3ADCCB65ADCBADDCDD4D7B73CF173148C6296390F0723BD37FE09E5FB4FEA3FB4F7C063AA6B71C71788B45BD974CD4FCA8FCA8A59411FBC03DE803DF68A59BFD4D7C7DFB3FF00C265F88BFB4A7C76D2356F10F8C25D2B43D62D62D3EDADFC47771C5682587CDE0097AE71401F6052F922BE43FD8EFE2978AFC3BFB64FC46F8517FE23D47C6DE17F0CDB7DB2C751D47F797769FEABFD17CDFF0096BFEB7FF2157AEF8AFF006D8F873E08BBBD4D4B5C992DB49B9FB05F6A31E9F75269F6975FF3CA5B98E3F281FA9A00F5FF002452579F78ABF695F057837C6BA0E81A9F8820B4D53C50231A44462908D43CCE862971E59FCEB53C3BF18B41F167C41D77C31617F24FAC786845FDA36C2DA54FB379A331FEF08F2CE40E809A00EB7CAFF6CD15E3FF00B65FED4307ECB5F09FFB5E3B45D5B5DD5AE63D3F45D3FCCE6F2E65E83FEB98EF543F668F07DD4F70358F13F8F078D7C70B1F997F1596A38D3F48F33FE59456D1F41FF5D45007B7D2F922BE68F8F52F88F43FDB7BE0F245E2BD58E89E219EF966D0A33E55B1F2AD33E69C7FAC3C9E24F5E2AEFF00C1427F68487E04FC2AD32C5B55D574297C53A845A7C9AB59DB09A5D22D739B9BA1FF004D047C0FFAE9401F4579228F2457CF3FB147856CDF4ED43C55A5FC6BF147C53D1B508C5B43FDA3762586C3041FFBFBD8D781C1F19ECBF6BFF8E5AF59DAFC76F18FC2BD55B53FECFD1741B5C4315E5B458114DFBDFF0096B2CBE6FE5401FA05E48AA177E17D3F50BA135CE9F697537FCF49208D8FEBCD33C39A6C9A0787ACAD25BC9F5096D6DE38E4B8B8C19A6C0FF58FEFC13F9D6AD00569ECA3B884C524714917F729BA76996FA65BF976D0436F17F7228FCB1FA55BA28031ED3C25A5E97A94D7D6FA669F6B7D30D925CC56D1C734BFF031C9AD5F2453E8A00C31E02D112EE4B8FEC7D24DD4BF3CB27D8E332CBF8E335A3A86996FA95A982E6286E613D5258FCC156E8A00C2B7F01E8769AEFF006B45A3E9716A7E5795F6D4B58C4FE5FA7998CE2B4356D1EDB5ED3E4B4BCB782E6DA61FBC8A58FCC8E41F435768A00E507C1EF0A34D6B27FC233E1E32D949E6DB3FF6745FBA38FE0E38FC2BABA28A0028A28A0028A8E8A00928A8E8A00928A28A0028A2A3A002BE3FF0F780F4CF889FF053AF8B56971A96A9018BC2DA663FB3F5296D658B3C49FEAFBF4AFB02B93D1FE07F83FC39F10EFF00C5961E1CD2AD7C4BA90FF4AD463847DA66FA9A00E7FE1C7C27F87FFB237C3FD667D22CED3C3DA4006FF56BDB896496499B07F7B2CB264BD780FED9BE29B3B2F8A5F023E3969B78354F01E937B2437FA85B1CC50DADD01E55D13DE2AFAEFC49E1CB2F16E8975A66A36905F69F7D1F93716F3A662963EE0D56B3F03691A5F8423F0FDB699A7C3A2C56DF648ECBC91F6511FF00CF3F2FA63140185F147E34681F09BE166A5E30D6752B48341B3B5FB57DA3CD063941198C478FF59E6647435F327FC137350D3FF669FD9A0F89BC7DAA5AF85E2F88DAFF00DAB4D8EF71149299BFD57E32F5FC6BE85D2FF64DF873A65DC32DBF83344DB6B219E28A48B7C30498FE088FEEC7E15D278F3E127873E2A6996365E23D134FD66D34EBA8EF2DA3B98F77933447F7727D4500751374AF8FFE007C2BF0C7C4BFDB53F68A1AC69969A89B3D4AC228FCC7E63F36D3F7B5F5FD795FFC318FC30B8D4F51BD7F0468A6E757904D7B2FEF3CCBB93D64A00F973E18EAB79F043FE0A35E2BF007C32BD1AA78735AD1E4D4753B3F33ED234EBFFB24B2C67CC3D3F7BE5FFDFEAEF3F65CD4BC2DE3BFF82636AD6179118ADB4BD3753B6D7E3D4A48E6961BA8BCD966965FFB69FBDAFA43E1DFC11F097C1FB4962F0C786F46D044DFEB3EC36B1C3E6FE55CF6BDFB28FC3CD7FC5771ADDE785AC25BDD4A43717E712470EA3201F7EE6207CB94FF00D7506803E6083E1AEA1F143FE09DBF0AFC27AA42D17C4E9ED63BCF0930907DAACCC528962B9F33AC71450F95E6633FF2CFDABD6BFE09C7F142D7C73F0A750D1750175178FBC3BA8C96BE2C8EF24FF4BBABAFF9FAC7FCF297B57B449F077C372FC41B4F160D1ECFFE123D3AC7FB3EDAF71FBE8ADBAF95F4ACCF0E7ECE3E0BF087C4ED43C67A66816763E27D5BCCFB66A113C9E75DF99FF3D3FCF1401E09FF000527D074FD4FE29FC0A9FC5114575E0CFF0084AFECBA8C727FAA324A008BCDFF00A659149F123E1868DF0D7FE0A37F09F51F07DADA691A9F882CAFE1F105A69F18884F6B145FBA9648C7A1E3D78AFA63E26FC31D13E317842F7C3FE24D32D358D1B508F12DB5C0C835CFFC2CFD9BFC27F07B5CBED5F45D366FED6D4208ED67BEBBBB96EAE5A28BFD5C4249493E58FAF7EF401E67FB4A5EC707EDC7FB3F46E704CBAC7FE9257B56BDE23D01757B6D0354D47493A86AF149E469D732C7E65E47DF111FF582B9FF001A7ECD3E0FF881F1174AF15EB5A5CDA86BDA1FFC836E85ECB19B4F78C0906299F1A7F65CF01FC7BD434BBCF15787E1D5751D239B1BBF325866B6FA491918FC6803C77F678F863A27C24FDBAFE275BF83E38ACBC263C3D6377ABDBC5C5AE9DA9F9B37EEBF18BF7BF8D58FF828C7C3DF03FC56FD93FC41E2A12E972EA7A1DB7DB342D6ACE58CCC6EA3FF00571452C7FF003D7FD575EF5EEBE0EF837E1BF00784EE342D2744B0B2D22F3CCFB45AC6BF2DC9941F34C9FDF273DEB80F871FB047C2BF8517F1DDE89E17688D9DC7DAEDADEE2FEEAEAD2D25FF009E91C52C8631F95007A3FC2897519FE1778764D637FF0069CBA65B1BD1275F3BCA1E67EB9AE9AA3A92800A28A2800A28A2800A28A2800A28A2800A28A2800ACBF10EA53E93A15DDCDBD9CDA8DC4111923B68DC09663FDC15A945007917ECBDFB451FDA0BE078F18DEE9C74126E6EA196D8C9E6FD97ECD298CFF226BCDFE227ED8BF12FE1E78325F1EDDFC2D8A7F86D045F68988D5CAEB90DB6DE6EA4B7F2FCB1EBE5673EF5D4FF00C138ACA3B7FD94F49C0199B53D4E597FEBA7DBE6AD0FDA8BEDFF0013BC3373F0CFC3B2C2756F15C3E46A374C498F49D3A5244B2C9CF5906628E3EF9F41401D97893E2D4917C1AFF84BF40D326D7A19B4DFED4B7B612C76A668FCAF3473274E2AAFECDDF189FE3E7C0EF0E78CA4B38AC1BC416DF6B16E92F9A238FCCF5FA568F89BC3D69A1FC1AD4F45B5416B636DA2CB670FFD328C43E5FF004AF3AFF8269C4E3F617F86A2493CD3FD99D7FEDACB4016A3FDA475F9BF6C2B4F86B71E18874CD2A6D12E3598B5292E44D35E88E6F2711C71FF00AB1CE7F7B5ED7FF2C7F0AF9C7C4B7DE57FC1537C396E4E3CDF87D7263FC2EEBE8DA00F06F1EFED51A9F88BE3CC9F0D3E1C6876DAEEBDA444975ADEA37F2C91695A1C478F2E4317324DFF004C811D693C69F18BE27FC24F1A785ADF5BF0F787356F0E78935CB6D264D4B4B96E22934AF365C6648A5CF9838FF5831F4AF35FF825CDF241E3AF8E567A84A7FE12CFF84BA596FE293FD6F95997CAFEB5DF7EDA9F183C61F09F5EF87107872E3C3E6C7C61E208B44B84D46C4DD18E497FD54B1FEF077CF1400FD57F688F88171FB5EEA7F0CB4AD23C2C2D6CF481ADC5A8DDDCDC1325B1222F2FCB8FFE5AF9BFA56C5DF8F7E2CE8BF12F47F0FDDF87FC2335A6B56372E9AAD9BDD4B0DB5D440111CA846638A4E39CFAF7AF2BD5ADF5FBFF00F82ABEA96FA26A763A75C9F0145E6DC5E591BB8FFD77FCF312C55F40FC3BD27C63E1DF136B92F8A75FD2F55B0B9FB3FF0066ADADB1B516E00224050C87BE0E7AFF002A00F36FD94FF6DD7F8D7F12BC59E01F1869DA5F877C77E19BD961FB0DA5C99A1BC8A3FF0096B1C86BB1F13FC54F11E89FB4D7873C19058E8D2681ADE9377A8497724927DAE036C62CC6231FBBFF0096D1F3EF5E29F13BF65FD47E2D78635EF16782F5082C7E26782BC65A9DD69179C62E7F7A7FD165F63537C05FDACF4FFDA67F680F861244B169FE27D3B44D5EDBC4DA5491625D36E47D938FFBFB401D6FC19FDA67E237C6FF00887F12742D3F44F04D9FFC207ACFF64892E2EAEBF7FC7FADFF00555B363F17FE2ADB6A7E2FD0EEFC2FE159BC43A1E9D6DA9E926CEEAEBECBABC7219BCC88931E6397316075EB5E6BFB2B69DE2BD5BF682FDA064F0BEA9E1ED2A28FC65E5DC8BDD365BB32C9E57B4B157D05F032DFC47A4F87AFACFC5FA8DADFEBB26A375246F1C7E546F6FE67EEBCB8FAF9783401CC7EC61FB4D4FF00B52FC171E29BED3AD747BC5BD96D27B3B791E536863ED27BFE954FC15FB40F8ABC65F1C7E22F83ED342D1A5D3BC176B1C916AAB7727EFAE268BCD8ADA48FCBE24C73260F00FE15E21E15F17E9FFB04FEDA1F11344D57169E0AF1B6992F8BF4D323F9517DAA2FDECB10FF00A6BFEB7FF2157BC7EC87E03BCF04FC081AB6B11C9FF09278D6497C4FAC798727ED375FBD10FB795179717E140185FB337ED21F11FF00698F84D0F8AECFC3FE0AD3229EEAE6D3ECD717D75E6E6297CBCFFAAAF43F815F13F5CF89161AF47AFE851681AA787F57934B9A08EE3CD8A7FDD452F9B1C9DF896BE7BFF826F695E36BEFD922DCF8775CF09DAD8CB7F7FF0064FB469B2CB3452FDAA5FF005BFBDFF9EB5F507C29FED8B5F87DA35A788E5826F11C36517F6898E4CFEF680397FDA13F699D17F67BD234C5BEB7BBD5F5CF104DF65D1746D3D4C977AADC633E5463B7FD743C5741F0B751F17DE68FE6F8B2C746D36E64FF00576FA7DC4972621FF4D24703F415F30DF78275AF8DDFF053DF18187C4F79E1D8FC07E19B586C6482DE29A5FF004A1FBDF2CC9FEAABB5FD8B3E32F8DB5CF8B3F13FE1C78EF51FEDDBFF00025F442C75516C2D65BCB597262F3047C66803E9AA8EBC83F698FDB33C31FB2CEADA559EBDA6789AFE4D6229658E4D274D3762211FFCF4C1E2BA9F80DF1D346FDA1BE1D5AF89F44B7D56DB4EBA9648563D4AD8DADC0319E7F766803CDB5EFDA8FC57E34FDA435DF873F0F744D0659FC276915CEB7AAEBB732C50C324A331451C517EF24CFAF03B5755FB397C6DF11FC52D53C57A478B3C267C25ACF84EE61B6B88D6F3ED50DD79B1097CD8A4E3F778AF08FDAB7F64DF1D0F8F37DF157E0A789E2B6F16C36D1C7ABE8B3498379B63FDD4583FBAFF0057FF002CE5E3A1EE2AD7C16F8F579FB6BFC36F1CF8235CD3356F87BF15F48B202FE4B279AC66F3307CA9A23FEB4763E59FF9EBEF401F63551D667B8B2D1EE65B38FED1731C4E628BA798FD87E75F9D5E31F8BBA978F3FE09BDE13B3D12EBC590F8FF00C33A9DCE9F73F67D5E4177E6D8C52C97D2492FFCB58BCAFE62BDA7C3DAC1FDAF3E38781A4D1BC4BE27D27C39A6780E2D6EF61D335396202E6EA402D44BFF003D258BCA97F3A00F57F833F1A3C630FC01D4BC57F16F40D3FC13AA6906E65BAB6B797CE896DA2FF96BFEB0FF003F4F5AF4BF0A7896DFC59E1ED3F55B403EC9A9DB47756E7B98E58C482BE041A4DE7C60FF008253FC41D63C41ADF88358BED0F56D5EEAD65935297F7DE55D1F284BFF003D2BD6B56F835A3F803F67EF09F8BF5EF1CF8E34DF07F86B411A8DEDBDB6BF75E6DCCF2C70F9623E7FD5FDEC47FF004D6803EB9A2BF377C0FF00103E247C3BFD97FE20FC707D77C4B6B0EB52FF0067F83B45D5B5396EE2D3A296EBCAFB54BE6FFACFFA655DDDD7C34F8D5A0FC4DF026B5E09B0F88B0C0F285F128D77C556B7F697F6D81893CAF37FD6FF00ACFD2803EE39FCCF24ECC6FAF07FD967F69AF13FC64F8BFF00123C39E24D034FF0FF00FC213756D691416F77F6B98F9A09CC920E09E3B57807C1DF84DF147E36FC68F8B3A447F177C59A5D8F81FC5B6DF650F89A5BAF69338FDCF95FF2CABBCF85926B967F19FF006B193C3B24106BCB2DA1D365BE7F2A28A6FECFFDD1F33B459C1A00FAFE8AFCE17F8AF77F0FBC51F05AFB43F883F103C45797DAE5A69DE2CD41EE6EAEFC3BAB49263CC8A2925FDD498FDEFF00A9AEFBC29E03F1F7C60FDABFE2EF80EE7E2E78B2DF43D26DEC26125B431C5780CA3CCF2E2E3CB8A2ED9C7EF7E99A00FB82BC0BE2AFED45E25F87BFB587803C003C2D0C3A378BAE661FDB17171E6F9A238B24471C5FEAFEB2F15E05F0B7F698F1FEA1FB2FF823C269E2394F8DFC59E38B9F0845AF5CC7E74D696D14DFBC97F7BFEB65F2AB63E257C1ED63E1B7FC142BE04599F18788BC43A5CD15FCB17F6D4BF6A9A192287F79FBDFFA6B401F6FC3D29F4C87A53E800AAB7E247B29440E2394C7FBB90F356A8A00F2CFD9E7E086A1F01FE05C1E0D4D7DB52B9B3FB4FD9AFCD908BCA32CD24A3F760F6F32BCE3C23FB1BFC49F0447AD3E95F1CB56825F105CFDBEF2E27F0D5ACD2CB263B799FEAC57D35450071507C3BBBD2FE0EFFC23169AACD35E0D37EC0351BC4F3A5964F2B1E6CB599FB2EFC1ABBFD9F7E06F87FC1B73A9C5AC7FC23F0FD963B84B5FB2F9918FFA6638AF48A2803C5353FD9DB5DBFF00DB1F4FF8A1FF0009069F1D8E9DA24BA1FF00651B1FDF18A43E667CDF33AF9BCD7B47FCB1FC29F450078378FBF63E74F8C93FC47F01F88A7F06F8C6F6D45B6A20DB0BAB0D5C751E6C4C7A8F58F1D2A2F897FB2CF89BE3978A7C117BE2DF1869A2CBC1DAB45ADC565A568FE49BAB98BA66496594D7BF51401F3D6A1FB26F8ADBF6AED4BE2769FE37D3AC66D434D1A57D8A5D17CEF2ADC10473E68CF22B775AF80BE27F18FC4DF096B5E23F1BAEA3A47852EA4BF8F4AB3D1FEC90DDDCF978865964F3643FBAE715ED1450079BFC0DF855ACFC2DD3B5D8F57D66D35A9B5BD6EEB55DF6F65F641179A47EEFFD61E6B9BB7FD9234BD13F6C08FE2BE9971F62B9BBD225D3B53B4F2F02F24CC7E5CBF518AF6CA2803E75F839FB2DF8FF00E0CF8A7C6DAA5978E740BB7F1BEAD26AD7093E81262390FA7EFABA9F86BF047C4DA37C60BFF19F8BBC5F0789EFBFB37FB2F4EB6B2D2FFB3ED74E84CBE64871E6CBE6492FEEF93FDDAF61A2803C2BF6BDFD8DB4BFDAAEFF00C1135F4A2D64F0A6AF1DE4A7CBF33ED56F91E643F898E3FCABD57C71A76A5AA7842FED3449ED2C3509ADDE2B696788C91447A731F7AE828A00F9B7F65EFD997E28FECF1F0720F0A47E34F06B47672CB3452FF61CB3006494C9FF003D62F535DC7ECF3F047C41F0C756F15EB9E2BF1241E27F12F8AEF229AEAE20B2FB2C50DBC517971451C7CF4C75F7AF5AA2803C1BE277ECDFE238FF0068BD3FE27780B58D1B4DD665D3BFB2F58B2D5ADE592D754881CC67319063901EE3DAB67F66AFD9D6E7E0CDCF88F5EF10EB03C4BE35F1A5E8BBD5F5158CC50911E4451451FF00CB38A31C57B0514011D022100FDDA0A928A00F9FB5BF837F14F43FDA03C4FE31F09F897C2C9A7788FECD11D1B55B6B99611E4C5E579BE646731CBF406B4FE06FECEFAD785FE2F789FE2278C751D2752F15F896DE2B048F4CB578AD6C6D621C463CC3E649CF73F957B751401F39FC1CFD85B49F865FB47FC4DF1A5CB417563E36FDCD9597FCF9C728CDD7FDFD96A3FD8AFF00648D4BF641F847E27D3EDEF34FBEF10EB57F2DD5BC92193C88A2C7976D11CFA01FAD7D2145007C89F0FF00F627F1D685FB0678EBE16EA9A8787BFB7B5F96EA5B0B9B3794440CB2F9BFBDCF35CD7C4BFD8C3E39FC70F04781746D7FC4DE01BAD27C3BF6796FB47F2AE61B59E48B0238A4317FAD8BCB007FDB493DABEE0A2803E66F895FB38FC45F8FFE1FD5BC19E2FBFF0004E97E0ABCD0A4B4B78F41B597CD8AFBCD8BCA97CB978F2A2F2EB9BF803FB3B7ED01A5D8E8DE19F1978EF4287C15E1FBA4E74E8659756D5EDE121E28BED3D638BB67FD6F1D6BEBDA2803E6EFD923E0A7C43F851F17FE28EB7E2E83C331D878EF54FED5B71A75D4934D0C9C8F2CE631C7975CA7FC32EFC4AF1D43FB42DBEAD1F8774787E28C718D264B5BE926962F262F263F37FEBA47D71D2BEBCA2803E13F117ECD3FB4178BFE1F7C2B8EF34CF014375F0E358B536DA55BCB88668A28B1F6A965FF00DA5157A27ECFBF0CFE2AF847F693F8A1E35F12F84F44860F16E996AD6BF64D57CD325D5AC5E588F07A472F5E7A57D5145007C010FEC2DF15BC43FB2CD869F25A69DE1CF885E08F175C7897C3FE55FC534577E74DE6C91799FF002CABA9D53E1D7ED1FE3EF8FDF0BFC5BAE68DE02D2DFC2DF6A8658E3BA9668A1F322F2659A5FF00AEBFF2CE38BA77AFB5A8A008E1CF9237E33525145007FFD9

update Office
set OfficeStamp = @image
where OfficeId = 1