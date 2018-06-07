﻿#region License
//
//     MIT License
//
//     CoiniumServ - Crypto Currency Mining Pool Server Software
//
//     Copyright (C) 2013 - 2017, CoiniumServ Project
//     Copyright (C) 2017-2018 The Merit Foundation developers
//
//     Permission is hereby granted, free of charge, to any person obtaining a copy
//     of this software and associated documentation files (the "Software"), to deal
//     in the Software without restriction, including without limitation the rights
//     to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//     copies of the Software, and to permit persons to whom the Software is
//     furnished to do so, subject to the following conditions:
//
//     The above copyright notice and this permission notice shall be included in all
//     copies or substantial portions of the Software.
//
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//     IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//     FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//     AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//     LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//     OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//     SOFTWARE.
//
#endregion

using System.Linq;
using CoiniumServ.Cryptology.Merkle;
using CoiniumServ.Daemon;
using CoiniumServ.Daemon.Responses;
using CoiniumServ.Transactions.Utils;
using CoiniumServ.Utils.Extensions;
using Newtonsoft.Json;
using Should.Fluent;
using Xunit;

namespace CoiniumServ.Tests.Cryptology.Merkle
{
    public class MerkleTreeTests
    {
        // object mocks.
        private IBlockTemplate _blockTemplate;

        [Fact]
        public void TestWithZeroTransaction()
        {
            /*
                coinbaseHash: a3291f854e60860ec74caf232ed34f98d0ff447dd7d38dbd7d451462b4b6f263
                merkle-tree withFirst() - first: a3291f854e60860ec74caf232ed34f98d0ff447dd7d38dbd7d451462b4b6f263
                steps: []
                final: a3291f854e60860ec74caf232ed34f98d0ff447dd7d38dbd7d451462b4b6f263
                merkleRoot: 63f2b6b46214457dbd8dd3d77d44ffd0984fd32e23af4cc70e86604e851f29a3
             */

            // block template
            const string json = "{\"result\":{\"version\":2,\"previousblockhash\":\"1a47638fd58c3b90cc3b2a7f1973dcdf545be4474d2157af28ad6ce7767acb09\",\"transactions\":[],\"coinbaseaux\":{\"flags\":\"062f503253482f\"},\"coinbasevalue\":5000000000,\"target\":\"000000ffff000000000000000000000000000000000000000000000000000000\",\"mintime\":1403563551,\"mutable\":[\"time\",\"transactions\",\"prevblock\"],\"noncerange\":\"00000000ffffffff\",\"sigoplimit\":20000,\"sizelimit\":1000000,\"curtime\":1403563962,\"bits\":\"1e00ffff\",\"height\":313498},\"error\":null,\"id\":1}";
            var blockTemplateObject = JsonConvert.DeserializeObject<DaemonResponse<BlockTemplate>>(json);
            _blockTemplate = blockTemplateObject.Result;

            var hashList = _blockTemplate.Transactions.GetHashList();
            var tree = new MerkleTree(hashList);

            // steps counts should be zero
            tree.Steps.Count.Should().Equal(0);
            tree.Branches.Count.Should().Equal(0);

            // calculate the result
            var result = tree.WithFirst("a3291f854e60860ec74caf232ed34f98d0ff447dd7d38dbd7d451462b4b6f263".HexToByteArray());
            var root = result.ReverseBuffer();

            // check the result and root
            result.ToHexString().Should().Equal("a3291f854e60860ec74caf232ed34f98d0ff447dd7d38dbd7d451462b4b6f263");
            root.ToHexString().Should().Equal("63f2b6b46214457dbd8dd3d77d44ffd0984fd32e23af4cc70e86604e851f29a3");
        }

        [Fact]
        public void TestWithSingleTransaction()
        {
            /* 
                coinbaseHash: 357deb5f66416ac7bd10d21557f50d358d85581c4c2e725dc1113cd277869a1a
                merkle-tree withFirst() - first: 357deb5f66416ac7bd10d21557f50d358d85581c4c2e725dc1113cd277869a1a
                data: [null,[53,242,80,55,174,213,162,52,45,204,185,251,93,86,89,207,225,108,2,213,196,226,105,86,44,36,81,78,26,93,182,160]]
                steps: [[53,242,80,55,174,213,162,52,45,204,185,251,93,86,89,207,225,108,2,213,196,226,105,86,44,36,81,78,26,93,182,160]]
                => f: 357deb5f66416ac7bd10d21557f50d358d85581c4c2e725dc1113cd277869a1a step: 35f25037aed5a2342dccb9fb5d5659cfe16c02d5c4e269562c24514e1a5db6a0 buffer.contact([f,s]): 357deb5f66416ac7bd10d21557f50d358d85581c4c2e725dc1113cd277869a1a35f25037aed5a2342dccb9fb5d5659cfe16c02d5c4e269562c24514e1a5db6a0
                |-> new f: da307cebe47b9c45046ef74cb4d800d8c90ad8bf1b542d501966fb2dae44b129
                final: da307cebe47b9c45046ef74cb4d800d8c90ad8bf1b542d501966fb2dae44b129
                merkleRoot: 29b144ae2dfb6619502d541bbfd80ac9d800d8b44cf76e04459c7be4eb7c30da
            */

            // block template
            const string json = "{\"result\":{\"version\":2,\"previousblockhash\":\"316b5be0c2cb6903170c1b470fac606c9ecedd149233eaabc1b453843ba408f6\",\"transactions\":[{\"data\":\"0100000063195b352abd3fbe585c1fe6f4ad8a3f4d6204d710c6e67edca5e9d885c45ad0df010000006b483045022016c82399242e8139c927e1812371e0c5907f41557f3598c6f49eda1139e74aa8022100b2c8bf60afc241f3927ff1557f8193c3824046c73af6a43c3be98c59906d8b70012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffff1b5eb62dc71ef8ff6a8996014c93b95fef85d611f775dbdf5579d94b340ae1b0010000006a47304402207cec287998926f9affed2a2009adde917453f56dc3b0e922ff42f2fa36424e4d02203763753cc226f199b75e3f63eb7fdb4ff5d86b5f1c5e1c7a54042d971110659c012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffff1e240ac9ead7cedb8f7a086a46712eb911f4a9b0c0260d06c46ec3cf0662b0b9010000006b483045022100ad806af12f4a56eb802f8e2fedc61c479182b6f90b33b6c1c88df3626e86fee102200a0d684219d17932b0c000c063c8cf5a8033e36fc612cf1dc7a0f4742651a950012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffff221a7929f9d1dee98057888a1c38ac17a9c4f43c452cd7bf8a68b32a29237088010000006c49304602210098b89ef5daab2018a9857931faed74a75913e88c25c1b6c99f8aa2249f8c21b1022100e94acfee313c1076f4259a8b39be7c07186b68649b6caffe0e7092efd0562bd2012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffff1cff7a13876e218fbd74b346f4dc5bb6ab83eb27979dcc2c3499e3c25f3a7ebd010000006c493046022100e2a1b1c52e7a3aca82167ec5c29aa935f373704118cf7171667c93ec97d4d0b6022100a0d136ec55ae908e45b9f463368ff5802c252fc408b6da91820cd0de1b5c4234012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffff2d3671427b296373eace1b0330a666093d8cb88daf9ccb73df1d7809130648ab010000006c493046022100b8de121372b545f1140ca79bd5a5c3cc5c5c4045ff80d0bfcb67866ddbaa49ea022100eaa28e101c2855da293589e7f2d2e29e984122a17cbb22ee4679f21c5479d5f2012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffff19d431eea5c51a1bdc6b2bc857ae6ae01c344f2212cc7396fec9ff5933f00111010000006b483045022100c478282ec6614f418b3120dc42e51e794a4f91bcbdeefaba3fae0fb51b85fcc302206b816fd61780313754a870b538c92786b8127cf3e3abd339087023d8ca44b6f1012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffff33958ff99855661de65816e47b0105d65790fe2be1f0439bc2f824e1e697a416010000006c493046022100f1dac19f73241c6e4305d7291943f5b296a75890781c331508fcf88e2fd6119b022100f2c4030d1c0fe182fbfd67f36623695c91749859014e378a014e5d512bd709cc012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffff2f6f8ae9d5c446d386e62000ca52bee90ac7904b555a5a8ef0f4476ae829e6e5010000006b48304502210099ab8b04dc971212fb048169548b3e498cfe3406bd1d8779542296083cd2ae7d022062d22dd07c02c848e977f9b97279c7c1b43ddabc58d30d968fbd29bd613a1ecc012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffff1de66cf52aaa7e3f1946bb25fd74e2e53b17b734c0688e9b2c3b410d129f2aa6010000006b4830450221008858704d6c542141075109161fd8e2727d36a44c92dba5aa4d17d3afa16a2a3002205b944ee78b2a4150d958d9612652e90ecde7a25ad615f8ed802a61a436a6b5f3012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffff005d596b0a30215bc1755cedeea7e390a21152a52866753e6b366e9db6ad6af6010000006b48304502201235d5c427e25a1a21212b7546b9a46d2de7d458f4f673a9aea45623796c2419022100e9ab00bb8e0852980fc85f7aa175317a062ec565b058714cb194a13dae978d68012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffff5773d10dc277506be4647e47f9a30d230dddf8a7ec07606a8f0eda5678bbdede010000006c493046022100c13935aad9aeb11c7f5bb2eedf336db7fa69c103b18d5ccd1f0905a0e2ea6ad302210088375f07572876aebeed1c7cb17fc2fe8db4e5831d5130fd2f26524ffd818eee012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffff026560956605b5e5427444d06104e077fecf48576c5d57ad39c2329a34165b91010000006a473044022035e60922c9b98787849ade284a22f514af93372320907c60f73ef930234d3bae022039c6c18d3db52ebc4c01268bfd6bc01c2f1b0f297b4394e6077519626296c8c3012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffff1733c8687f0f0fe4cc9041be77b404b10b026b640ee94070516d4ab217d1bebc010000006b4830450220412c10ea12c4ad7ff4c2d73a59903733f6fbe2c56d2ff3ed935fcd6f00803f360221009941fcbc867fcc3acd3bbf45bd15401b8edc95661db1a6c5e7df36f6ccd4303d012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffff0eae7df30f1017124f71a99d4c1eb243349b9fb77f78b58c193edda937394f84010000006b4830450220158157dbad767d972417f68bc6e39ee1b2ab89fcc2504207de15b0363635c365022100f5eeb056c2f7b9ae98bfaba82e0127ffa13c3aca6e43a794e2c0a7dc98b1f890012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffff0f111c03656208eba0e9087f986c3952e49df6766a3b44597ed3ce45de647ef7010000006a473044022018732a69c4613ade7095c10caeb706a2afb9a9e4b4783c8392e36d6b623d459802202b71c498b9bc5e9f549b389e895c513aad5851677fd15358826e9dfa208caf92012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffff175d447812a6744a63c8d13759ac35e9947c7c3ca82b229bdc1ef379ceef4f8c010000006b4830450220432b55dbf6e25150d4385947c9f7da2e0962541ee8456bec179a24c6741a4916022100e45708be4144917092e4e9ad9bf7e0673856e2880b2e760e34b6dc1fe3ea1117012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffff115f506115a72067295c3d27a4c6ab910500cb53cad8bce7d6cf7cb0df098f9c010000006b483045022029d4e3a5fabb6cb590d327101df28bdc75b2a443fc6db55b98060b809d145d250221008d0569f2b2a3171dc45b254a842ac3bb4612ea1fa08e0748209c5cc045a9c80b012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffff11ec5df124632e82b6262309a121d9ee39664b4a54c8d5d0ea73261967983367010000006b48304502207980e56ca668cff0729e1077f25ebdfa8e97e9b00f7015685be5d6240cff1a8f0221008cea70e725f867e15acba24a8a0f1b3be615b8d0b475391fe579315242a3e01d012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffff3a09c986ee80b521dd5869e742f61e62943a88a10614d5549976656f00820b53010000006c493046022100b48ef369a8fdea0cd145b5a17693cda794ba6360e7ebb028f3b34a5b6a2d86f2022100fa936c8b014251d394359cbe307a958d286afb1c47ce5e1c52529a0f2384b613012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffff3ce1cb7b39e1ac79d7153cbc1edc23d9f999db77068c4b499af81ca526fd4ef0010000006c493046022100d171dbbfc07dcfc2be0a3fed6b4d5592e0013283e90a3d9ec3ee39bc9422a93a022100ddb45b276397ca821db6e073e884c5af9a278aac1e1aa582b9b7a60c34acf472012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffff4c029423b5d07f2be490a314ec6c966a4811de1f834321858cc8d62d647c8d6a010000006c493046022100973f313b0a8441348463611dd1c965a5111cd183805829c454f99b5485663e9d02210092d8f2aa26db89ec248c1a8809705e4a66ee0b1987f659b2e03801711b561526012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffff37103393d35b3b0099047b9a007f97ef69d47e4b2349739068ad8e4a6577d137010000006b483045022100efccf9e799b4daae3fca9db282ca2792759afb4d174220626383cfec1817c998022037460fb9df694b8452cf321cb08fa5c97f077c0de0e0c36bf9bad46c6d6b6847012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffff3bfa34812fd69cf23805c66a46a94e80e75572dffb8ed80bab264a2c232cf92d010000006b4830450220304a938bf6cc79c46649f13de99c0e78a9c7c05d18f099ad3a1c60cbf19044de022100a086491d02816288bacd780e3e7b1af7f18d02487c9377905936b7940865d178012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffff446ddc21d05e11646c1b748124a93bd7196c9d6868b0cb77360cb5e562dc159d010000006b48304502207e9423eb4eaa78bdf36864e81b5ed438fe085be376b19a5226324779c5e347e8022100b3ef66370cbd9378b46d12e635450d2df1d6121a5928ef0c317768855dda4c9b012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffff705a9041175641c8fd3ac7be4dba8e0ec889a1b15ca1e7ba655d33dcaf8ca0cc010000006b48304502202664165ad19018bbe91c539e4b0d03c3922a5846cfabc3818d73d24be036961a022100a09a2932ac1264f54664cb27b9e003840385de498cec1d29353c3ae8fb0dfeaf012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffff61ea8bceccafb15b49ad38a7518012a3fbd7a158ba0717292f62c63e54ddbc4c010000006c493046022100f6b8c0d72acd9a3e81c9d1ad791895933d5342a0d1df15926e62d745439b3827022100bd1d9dbdf6db3bbb86b3b18685c4725901420bc5b22eb6fd776dd33f70af8aed012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffff61051200ba0c3294f64e6ccc2e2d9f3b4cb758d9962ffc825b087957016aeb73010000006b483045022100e5887f575fe9259b217cf29c54dd5d6e2b95748ba23a23541cb0dae75c31cde8022047df6b89651f5fbc5d97d4c10f347cb073cc27928b1c30fa72936c6d081c1658012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffff70a1ce46c9b972feb1e8f8d9ae4fb0afee18b2c48596c2920bf25bbdcbbcd8e6010000006a473044022062f5fb816804ff849fe4897658b0c01769e5b47ff952b42c1461b5cab9a9bfc402205150d033ab1a7e1da317c1e5d9e1f8d6112fd35db01c0af1e464e8dc39ae0290012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffff72c66df0450e8555d906934bf9cf84af53d31f19012314013db8fd024c25c152010000006a473044022068b747cf5a8ec630eb267c6d1e81e23f9aa5614ffb88214808819c7c548fffeb02202c227c8b1ec81f96e94d38139d020d14af5821128de0bb09322548545ba871fe012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffff608a9a230e3286e1affde4eda07d5f4eb06577f96d82869af2fc0348e7a7f52d010000006a47304402203441e58e4b699ab9cb498dacdd6f1f08358006a25974003155dde4352059ff0202205a12b022274057b5420a2e976a8b2afdadcafc4906decf015eff9a18897c44e6012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffff5b12511ee709c58d2708dd6c179558366209cbf3453e62cb78dd332fc5296c0b010000006b4830450221009901e4d340046ec76c4c93d2c5f7600e26c580854cb9488ecf3ceeaabc0d3e2d022025a5d0022b1c5814bff28d76a0f1b4a736ca93804614291971bbd2adf3bab087012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffff5a6188d4fba54151efe6b724cb6e7f90c2ea867c53e1b6f2f093a7c2b96b161b010000006b4830450221009c34621e02d38635fb1f96e8f1c1ddfd675d662213b88982977ce3d8be209a160220795f6ec9395b28764f59561d944e007d5b1445079cc9d1c970676f7faeef8421012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffff5b91edad5e98b3dba4f2bd6615ced2cdba2f3d6a6e37ba6ab085504a81902182010000006c493046022100bac8e11dbdfbe8eb35ef814a156ae927fcf45d640d9c5adbf4b8dd15e6c040f5022100ce5099d3fc2d363c14c64a849598f6c6e5659c1551e5157e4bd1d3356ad51433012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffff66c2ec4b69802501dfc0283d23a66b10a4110f9769006a1aed58910797264276010000006b48304502200df5287ef25a9d0c3c28c203a2393fe89652063e8f20525476c45d4d3390cff7022100f0e820ded05a4b95220de80fd6f95a8d77392ec2b4706492e9f9ef85aae87a6e012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffff78447b663dc32c1d4e288eef91a0dafbf3690e83b2821cff865e5d389cf9d4bd010000006b483045022100ab3ec3cc12346c7096162deabb43f36e2ae997312db361b22191573cf067fad802205bdc7d47f0e2ccbe2e7a922d00dfd6c398c81a59f33fa520f24d6b537414d696012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffff8010ba7058fc30d28be4f07c3ebe7cb2954fa2a01025ae8f72d12e7a86b370c3010000006b48304502207e6f181e5581e802ba34b1a00aacbf2bda88b0b52874ed69646871bd414ecb830221009c65fe42bf1bc0fc76e87cab7ac58ab38e48f1ed1809709b6b8c18e02814bdb7012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffff8840f1bd20c0f050d276744c23c97693c513f3303b50252c7da5ec5a4689b520010000006b483045022100c827c8317afd8b555eed62893cd990166cf7bbc2343e0a055a73529405967be702204ebb44097a9f33e8e9a98686cf099d74bbff540047534a6dcf15efeb02096ec5012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffff89551fd7c7dd51b9a6e4aa9830caa769862c24c11ef453e873e5295f73cd5b59010000006b48304502210082ed1e6cecae323521bebb9f5ee5f88f6f82a77100d05478b8c78aacebc5913302206977e474f3ab6a91fe70eac435ddb00bb64e0d47e056b85f37b36bc3a5ebc561012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffff79da523af49d7211da88d7166cdf4e594c14da0efe80d47d89d3acaaf88c72de010000006b48304502206573837a896f1b0e853ec7faea2da7d248fc91961b06535ffe6e6d19ceb30743022100809bb8015fe76b7f3e8553bb789c7ce896c09ef05d9920d2a8c667314556f5a7012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffff7aa2b162938bd59068061d8b6bc65c30da37d3edf66517bdcb8736e457aaf6bf010000006a47304402200cf17df9169ef6c29b233939bba1e7964366e74e6b18f66abc099f9e01bb7fb9022048c950c46e1d438791b66c0ec2139288f2d948b154561fcc3f2d15f1f55841d8012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffff788e7fe33541cc3b6253b8bc074b2de502ef48bcc3be1b041454989a0732faa7010000006b4830450221008d9dfe0522db8779b43391b2b95caf5d59e96b9b1dd8a1f4351a09fae14992e20220042b4aeeb47d0215f714f1abf1cd3ab018959ecc4a6255891b1fd46b34ee31ca012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffff81eb73622d1bf94844741d7986109b486b004d99e7a98f21019820a800d432bb010000006a47304402206f85b486cd1e6c395a1ed9bf2c6eda62a23b3463cefa6c61312a6537b5c74e54022070372df27041049d65b54707cced64d69a81a8f2aeb456a5aefdd8cbac11665e012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffffa33c2df50f5bf42356e0d41c8d730970ca2685afea1813da6c127e63ac604815010000006b483045022100c32f933993bc01f9f7e0e1315ad2f0cad844b1047257494c6e4f3b5a5ef16aa802203f34a186bf8bc9fd3b33a0c9a9d1630d675b6f99958da767a3c9b3836be1e54f012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffff961e8bc648b0fd248a310e49ce3ff329269fcdb53aede26938fb307012c191780100000069463043021f1bfb9005655b46a1fcf4688f96ab8a7e4fce4abbf8845cdda3328e2d54ce380220074f86a722aab68f15807b36c2f790b9c0fc7a01e35db2cbde80503011d39ee3012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffffa2b29866a296c028ec5abb9b50a1a09f57a120c1e35392f922d3daabbf40c463010000006b483045022100a83b1e3cd93cf48a19fb646d9a4a422b4577b0a487996256aed1071cfb3a80c602205d9adeba62741f973a5aaa1a371f0cff878fc7d7f9eb44856f1416be79b00b68012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffffa80d391ef8280b04aa95593f0fbe7cc83b09687eb0b4d10266c4ce73729a313f010000006b4830450221009d5ebba2c9f1468850fd447fdbd513961f01a86aba96f286e670ef1dd50243e6022010a3a64e2a1dec458f8f120e931ea4a99532000b3f77dcbd24aa2f0d531ab4ef012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffffab2fe4b89103e95e5b2f67b3cdbce8a243cff7b1754de10ec11e83be76a78ef5010000006b483045022075c77cd4d2de25c9ab384571a7701423eb2918efe574b1ca19f4c42c15c8cfb8022100ded02eee8ae95bef5a1f43708e69f162a72cb8039136cae4d2def71f078531c1012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffff9c992ae24d265aa58c8ea89c33235247b9db317b3602d743e57dc3b4aa041282010000006b4830450220038a227d2ea18a1a6c77437ef9c9a8ca4d513debe1b4368d81089ce9ddbf401a022100aecd51022645fd1f7b2679f786ee49bb4ea9c6f2bc40652af5c9763bdfac31c8012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffff9e30b763af5f7bd0a857ceb3706cb1c8744d851df4f45949de09e2708018b4f8010000006b48304502201ca2693590cd2c16a5b5b454a3a1935f8a00c093bf071246b90fa7e5e857ee56022100ca98659317bb502ead4eafe2c2ed1f00cfe00d3a70166f039e5da7d2f921349e012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffffb0d5dba0f4c2291999f9fa45c0a54d9a57083d34980de6d895976a8d401f272e010000006c493046022100f4c6a16f59d3b878a2620a4870f02428aca8155c328aea558f3b99f5f1b30797022100f141ed86e8b841dfaffd7f54e7556e7f895e0ed9cca650e438c57dc94b95c96d012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffffb815d182d1182b75ab8739527da8addc63c843e9c466f81298d318eb79bde8c9010000006b48304502201af4d9862743d9181080aa013c262c4da2c5bb9cb00952b321ad03e06dba98e5022100d22ec8b33291063078fdd8fa9faa0e6367f035e80593ea52ba269b2f2affad3d012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffffc01afcc0adcaada2680a750b00985e2cb9ef3ca2c727a38d54e6bf0384a8b75c010000006b48304502210087c9503c33ca8672c94de1b99cd1b14c673e38fea4a88c4a1b65308dbe89369c0220033bf0b196d56e826e3f6280d0aefadf92250932539804ac7664eab53676db3f012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffffc69d692943d34fdcb4b72b742515ba482b70b23200a0054517751580f0251982010000006b4830450220761501fe2fbc6b569531335ef64cad4b913b2eee5dc7dddbddb9882957844e74022100b7c4f643baef94e10b3f8faacdaa23aeb4cd0f09507600ebebc55e226062f6b1012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffffc677529bfdcaf1b95b38f79f8a5e78a30536065115cc330743564032ed5afa58010000006c493046022100fc78668b7de8f50cbf83cec3344bdcc98cfbf6de2a808850d7b3c047995326ee0221008912e22b55e3ef89d566bfa3f3a4096b2d229ccba256deb2d372bf7b13109b8d012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffffabe58957793d8f1fae08b704bb1a12066a11b94951b0de01e1c67e796f5b0bbd010000006c493046022100a548c8721d2d101eb850bd6d6d51ed305cb21a64e92434e6911399797b63dd1c022100bda738f38c3177107e443c300f1fa13cff95cb0598177e16a0db51bbf007b755012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffffcd222b4d82e9e48f03fb5fbaca4a41ddf8b1267e24a59ba0a109df7ddeacafac010000006a473044022050675a94f398e8b51691297b06d9e963c56299d8b6692010b6561f2f15bca349022001e506e314e29ef590e602566f0d605deeb8b44ec8faff3a75d36cbcb37b9289012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffffb1a4c670177453501d41a203fab17eec973780abca72b507fee6b7cc911ccc64010000006c493046022100a693b87ee79af7aa40c5482fe74ca65a0b6149970d4efe812864ede33c433f2c0221008db244fadba3bc18ed491400c99f38d6066d1b1c5a4065808254ddd4423e9b84012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffffb6b75ad8050b732b0fc327cd625d39660fb2aeeac123ce0dabc68726f9c01d95010000006c493046022100963d7cd31db784facd19de59e19131092a5ae6b4e3570cedbe7b890ae28177d0022100ddf6d5cc8773abd0b996dc583bddf010a348d13f8af1091f659c25ebe3962c7d012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffffb9b307a5622326e8655a8602c9dc46004cb7c53e7e67030461c6cf1a4dfd1e88010000006b483045022100decc1c2c34d529d60a8d1c1be0ddf484ddbce3ba6034e5268847e5ce7829923d022049278e72504ed70eb3fde47bc0b162b634d153c2ad6c43c1d6b09952ceef1470012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffffb03b4415511876c2fc38f0ec2c8bf10738890f3a41de5ae610bcbc8e86499b9b010000006a473044022077d8f1bb67a74bebe46b84185a0fdb36120b62f7622c117094c76f2989a46e96022014123f94075319b8f0102728c7315d56c3fa1a22ada7a6f4fe81a9c8214686cb012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffffb9d748aa7fdb578b24ce4b1dc1d85c3e9db6c6731d5c502d69816bd16e83632c010000006b483045022100b9f8ec0914693d3c04d5f42e8464bcb58a4d1375dfd90f9c2a1f07d28b3a85030220292b9a2c01966357d1bb3a48f539983ef3c1bd6bf334e66ce07acb7fea71442f012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffffd347a768891f6b83ba58c4a32dcc881ebfafd0f16023e1381eba6552cf11ed99010000006b483045022070051ccfaa2fc089ee3e39f1ccaa58d94f2cd40ccc4ec262e032dfbf0b29514802210085eb61e42f863642bb3a007c61e5a5e6f10fc9cb9ed22c21c6c2756b14b0227d012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffffd8da01e528dbd720cee36f75a01ae668064177a159628be5268e5cd661010f96010000006b483045022100825a22b9902bbfa85f566475d942d4a35b4e6bc602c21fb00d801c6f716217a202203e48fa7361b4bfae2290a52ed60771a14641767befc77335d7e2686b5342c56901210385cfa02612b467008b2ec21d844f14180c1703589b22f947f390c2166c5b04a5ffffffffd96a3d5d75c8c88d510f49902c31f714d96f46eec9968dcfe5b36d54454cdd06010000006a473044022045069157d124d7779a2d8ca6aee699e95152474689fcd49bc4644dbcc064625402201b11a9d793bb8adc083c3fff6d1b75f06c2dd9c223ce12200647887acebb10dc012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffffd201563abc564f03da3638dfdeab29847bb3882a3ab1fbafb8b68a3d6a01480e010000006c493046022100c550e8b7045f4f9300758208096ef9e49c8261c4fc8b22c87a964e6c6b44b549022100babc144c125ff5c4c74c574cca5d4357a94a37a1029309f59bbcc2a73ffdd0d8012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffffe6673929e7ab2d3ac05dc78a00478a0b218c3ec040eddadf3a18abddc810cf18010000006b483045022009ad3caa95c6aff101858737ab81f24e7ed25c0ef7d20a53b9ed0bf49195a084022100c79b3b3290da88e7e1ba63c53a03ac9b720256a3165cadc04f3f3448477216bc012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffffdce3ac7cf0a24987d34929cd17e3b539a07c85173da3a124410f8b98d1799950010000006a47304402206293abcd74d01e9760a389b725015fdcff225c1107f7d3b9224d56993f7a544602207e0a6ef39041b54c4e8b5deb48a86141898cd8024944a5dc120dd07e66cbb15c012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffffea77426309196a013b691bf66f140b10833758dfcf9402b80121e6bb572c91e0010000006c493046022100ebc7b68135899113c099ce0f96872ff28d45326b1b14673c296158fc098ec95b02210081f91954024f2e39006cc0e807a6334d0cf6b98ad72cc13d55a7a75c9bf5b82c012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffffeade1e79f22f95852e84024cc5d4b46ed7ec158f127bc3347d8e9e2d7ac9d899010000006c4930460221009243f3dca6d92acd61e4dde0157c1357ad99bce758c7a3d88c542e426583f98602210091efcfc5de2f66e4b69f72fb38e8d32b6e02f666f10451b4d3cc191c7accb8e2012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccfffffffff2ba406254b1eb86ab9a87554144871f77d1f9cc7bc190548f256d352e4e1335010000006c493046022100d1e9a9491f14b61e25eeba62f505213b0df48b965a455b9f91e88aa4b65bffcb0221008741904e389aa2cff150f2683db6d14398c9e89f59ac1ef3a9518f63477e7d9e012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccfffffffff226b799a7ad0c645bd8cf0df61395908679d65eb72d895d79f4bbd4b737d24d010000006a4730440220384ad7ec406f411c7e8d33b65152e38d1e72432fdf71c08605d3f1b53ef1868402205642bcb33c2447e4857bdc7dbfdc3756ede4c20c6fe7f660dc280000c95c604c012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccfffffffff3f88482b544b3ece07ae748dda78e7861b8da8fb92329df975d3d26e45ce324010000006b483045022100ec5c146f75139bf048d1dee3b77fafb31a1f19cd3ee1d08f227e0039283a862802206851f10a0568b6b28bc7af6e0b6b086a4cdf5ad1a56dbd9df2fd2665aa2370c9012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffffeafbfe205e452a8ca544c5ca63863b55b8acd8ae33a168cb00dd08a953676706010000006b483045022100fed6070b94e60b4337e8d018ca5fda0ae7efd2197b48526b89e86cedb5b21caf02204c4655a90b2310c43dd308adc261d24d950efca0ded7a2197330aa77ccc981ac012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccfffffffff0170ae72af55db98b3c7d78d09538a871e5d0b31a65ada19b293259a3d67164010000006b483045022078779c5539c226402b63211980be8b0325b6e74f846297e1c11510367697a454022100af4334ddefe94152e2ba3bbc9f05964b4db4298a93e84a679bba69a271f45f4e012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccfffffffff17f19331009e8ac2f04b165b71c1a558d1a81272f0f315cf8903bbf77284085010000006b483045022100a2407d564516f3bc6dbf7c3462b029986f3628fccf91a98602b25fb3700cc12d022078a6b1f923b3dba1b02a7ae64c1a465133260d31150e50954bfa622cd4ddd52d012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffffec0bad45b4334298df07b7de6acaf125e33cbe266a397d4c65dd4c6363be8aec010000006b483045022100fddda87f6eca7e8936a1698170c5f9f0d57f7181003d78290b9cb30f770ade22022022fa9fa100f19f1793712af460a229586a6e226a2b927cbbd4cc01c315570d2b012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffff54b6354f3e420d475074964b7a663564254077f9f720f9b82dce11f98342321f010000006c493046022100997cb1317be6b96830110cd2b31b5d38e50b56adb0ed9f5bdcd961ca69fc453c022100904f131f482c0ed691606fe47db95b8d9f03ae4821c6dab62ba8777cd1b3c670012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffffb4dd423815ded972f4c7c5423ae27c615be28b3b9ecebb37fc7e88162ed1c787010000006b48304502207f1c2a1df061f505df7ddf3ee0601065aceec8afa1858e13f69dc840e8fc904c022100a356119c6403c52c4a05728ec1cc277a774df7e7f1d29975525ca9d849b301bd012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffffe6d5b3956b61efdbcc772bc376a15391ccef2d84a276f91451895ef42dc93653010000006b483045022100b27d41ef820c7b4a15ba431c07bdccb8c471e2c8c9dae0dd6e4d41a6c534561902207de6feafd6425fc4a3d6df7f1aa7ffc549419d986c2be51965fe33d07e603b67012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffff0dc3e03b48afab770ddc15b067ba1faabb927742169f9974354fb51c034f2b33010000006b48304502207702d43b357841eeaf443cd3c4c9f54ce304c4f4e46c4edf4824499dcbf80671022100acac4286e633c19d0ad6b7751a567b872afe0d4c82abbab3ed4846ed00d45036012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffffb378134f068b0f8559e8b1ff1c49bc4ec599e93d22da663712c58ea64abb5e41010000006b483045022100fefa5510b6a07a69e397870376b9f17cb46a1a63b46c7bcd2d35addbfc2a739902201eb058c8761b3bf4f9eae857de7c4af24fd9ce8ec84f4fb2ebcb928466fc7029012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffff0bad2d8248323c440c356dccf7f6aa6ee0be2b1dac920ada290034eebfd53b35010000006b48304502204a7db829c441598c6d271ae28f32fc76ebdcd6b823ce2ddd7b4e907b617b592f022100cd2f508633136051d0a2eeefa46f16d4329ddaf5dc7c8cf19e0ac205ab0036a9012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccfffffffff439824c94fe03d979d6eff9ca31ac2ab0045fc54dac7c2781ec1ccc47d58d10010000006b483045022073f4f3c508719879922fe6cbcacc133050bfaabe9a16aa1b03d760b27bfd3a18022100d57108a9f99c426896d71a2ffe6f5d85e862cf3631906620203bcb12c897bbae012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffff6606ba6b364c6a4816fd15209ec490200de4da35e93de8f3c3c36542e93cc2b0010000006a47304402201d43c6aa1d9fae385af1f0fc1b189cd7e326ea25f952daf0695d84dc83d1a3490220144f68385529caafb10ad445f4f9e3cdbabd4f1c88c5d341b40318581d206420012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffffdb056164c83ba2569509c9761bcef3e7266c70b826d9f0cb63b062c92a2a630f010000006a47304402206b4966888b6cd0ad26bdddce57be638cfff477fd55643abf90201aeb6cf7518102202acbad466967580d89a54d0823c26d0aea39388922e916fe0e91a3415b7384c5012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffffccc8acc2298e4b01ec7fe7eaed332d8a82610fa1965adf8b0e7cdd5192034c55010000006c493046022100d978918a014b1d408609fc3e081b000992923c95dbfe0bcd6a70cb6b41cf719f022100f5f02a707f3ca9bba583db0e22190ad1f99f5bd97935ad9fdb48da218034eb18012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffffcc134060b6bc0851a20a6d47b5c788c56614e74ced08154b94f2e1c755874d56010000006a473044022069934d6b4515d41723604ab467cafe34304bf52c53b13809923e2f1a7cfa48510220690a32f5f68e6c87c5584cd8c018cf7567eeaa618e5974dcb08f426406a0e00d012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffff9b59e74d04ea3b428bca59c1eb34f1ae21a53dbcdd54c3c6c5b75b7655ad4dc0010000006c49304602210089056bb4129736014b95bda5988ee5a4d113723fcc08fa606569fbb9dc34c1a8022100f353c96775be055d8c68678012c7a4def1b4f220ea70bea976291637a219b929012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffffc48abda00e26bd8e1f813e64f39a40010fabfb3988040b2dd1b72d211502c917010000006a47304402204a0e02b1f2d5513d620edda6a3e983fab43832ce3efc8c8a4468cc396089746102206885e2bd9c5b08c87f77c5d22cfe12416e35ed6f047ab83df02fa33b84d6527e012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffffea2b5144ea42da02751b88a07fb983059bfce563d5bd71fd0292aa45098eec9e010000006a47304402200622167caaee1fc5a6953ecceff151c3fc77b99855733554d84026faee01a312022006a6471f50c72cb240d3bb61c720ea9b16b0c6149edf0c1dc3dce02c21e77305012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffffd8620a2c45e2fa402c5dc75059ecd5349c2bcabe287b3dd9b7ef6d592af14e4f010000006a473044022078c96b159f587a7e0b9473f1a67370a976c85b347654c86056d2a6fcb08a262f02206a811f7060b9346f03782d15d33ba7c11875096d07c9bfbef04ddddb050b9da0012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffff7ff170088cd616d7322d040bbb5e74dce990fcdd1d420152bcb083098e3a7d25010000006b4830450221009bb8365378f1a01833d69c2461965cb697ead2c4a5d90a008483ead3128a034f022008c5ff26bc45a1bb454d95cd9f1cec33dde4555fcd78fb2510f9a497c10ddbf8012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffff8c36bf255d340502399ae6ea2beaabf9de082606854ed101e5b4ec1a866ba2be010000006a473044022010645fccb74c108d9dc96813f78e5948fecd7034aba99aa6cbde48f5e3b5f75b0220686ca42bb7ace37a7f4b626042cc8e1ef70264b5cf58e1e73804bcfadce5c763012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffff00e20a74d15ad3d267616797e7b7a13715e62e765df57dbc2d3e916f51aaa170010000006b48304502200328c6af90e781cf17325d5ad4b6cf1fc9b40304682e8008964e891f74722967022100c6a260bf2833a29d8aa10803f40a6f85b71e3b028d482f68f2f6903fafc7e72c012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffff5d33d2a82584bc7aa8f3916d16e5967672e2e085178ef91499fa55d24a5b5762010000006a47304402206d4b818d9b953352b331acf16be14213d7c4f546e5bf8c90e2b3a16be1ad9a2d0220302bc2000a83fa19f55b2dae946c5e06383bd6775a7ce9817ebccb6ab1ccaa25012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffff69739d833a29bcb50c812eb92cffe3769902e067e1ba79957f32d732bcad6196010000006b4830450221008d1eb48a55c8bf452a89ed482a559af3f7325a410cc3f0f76e76a99e65b4f18b02204a58397e0ba0a17ffbf4af30b643a9e47feae709dd63c45d49e80acac3d5bc72012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffff6de7e5dbd1ad1a02abb0484ec67a2f1431c26e873ba68212d83319354a90dde1010000006b483045022100cee2c505c6e2d684d264e7133ed1ef6d24adecf933f0dfc5d3d93f5cd168665502204846e6a8f7be74a9e6b593df660c5b850d2bfd0521f6e03071b48aed857c8d12012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffff4d1a673e06a9e1e67787ca962bba031ed67942fb4fc30225014854f886550945010000006c493046022100a0344fdeb43040604af61faebbc93df7f9c272382d95636ca78d5bbe0e500a74022100d2af62e06b7d5a334bc3b8dba64cfb65a54ac3f6ea5cd516b94a936d8fea4fde012103d4df0e8aae26f39de0d1422322027aa9fd557fc233f5b9743ebeb73fdeccfdccffffffff0200111024010000001976a914329035234168b8da5af106ceb20560401236849888ace6a0b202000000001976a914abe4fe1979a61afe393558d2239ec5b8d40239a488ac00000000\",\"hash\":\"a0b65d1a4e51242c5669e2c4d5026ce1cf59565dfbb9cc2d34a2d5ae3750f235\",\"depends\":[],\"fee\":1500000,\"sigops\":2}],\"coinbaseaux\":{\"flags\":\"062f503253482f\"},\"coinbasevalue\":5001500000,\"target\":\"00000048d4f70000000000000000000000000000000000000000000000000000\",\"mintime\":1403699336,\"mutable\":[\"time\",\"transactions\",\"prevblock\"],\"noncerange\":\"00000000ffffffff\",\"sigoplimit\":20000,\"sizelimit\":1000000,\"curtime\":1403699784,\"bits\":\"1d48d4f7\",\"height\":315219},\"error\":null,\"id\":1}";
            var blockTemplateObject = JsonConvert.DeserializeObject<DaemonResponse<BlockTemplate>>(json);
            _blockTemplate = blockTemplateObject.Result;

            var hashList = _blockTemplate.Transactions.GetHashList();
            var tree = new MerkleTree(hashList);

            // tests steps
            tree.Steps.Count.Should().Equal(1);
            tree.Branches.Count.Should().Equal(1);
            tree.Steps.First().ToHexString().Should().Equal("35f25037aed5a2342dccb9fb5d5659cfe16c02d5c4e269562c24514e1a5db6a0");

            // check root
            var root = tree.WithFirst("357deb5f66416ac7bd10d21557f50d358d85581c4c2e725dc1113cd277869a1a".HexToByteArray()).ReverseBuffer();
            root.ToHexString().Should().Equal("29b144ae2dfb6619502d541bbfd80ac9d800d8b44cf76e04459c7be4eb7c30da");
        }
    }
}
