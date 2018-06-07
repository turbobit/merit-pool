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

using AustinHarris.JsonRpc;
using CoiniumServ.Jobs;
using CoiniumServ.Pools;
using CoiniumServ.Server.Mining.Service;
using CoiniumServ.Server.Mining.Stratum.Responses;
using CoiniumServ.Shares;

namespace CoiniumServ.Server.Mining.Stratum
{
    /// <summary>
    /// Stratum protocol implementation.
    /// </summary>
    public class StratumService : JsonRpcService, IRpcService
    {
        private readonly IShareManager _shareManager;

        public StratumService(IPoolConfig poolConfig, IShareManager shareManager):
            base(poolConfig.Coin.Name)
        {
            _shareManager = shareManager;
        }

        /// <summary>
        /// Subscribes a Miner to allow it to recieve work to begin hashing and submitting shares.
        /// </summary>
        /// <param name="signature">software signature</param>
        /// <param name="sessionId">optional parameter supplied by miners whom wants to reconnect and continue their old session</param>
        [JsonRpcMethod("mining.subscribe")]
        public SubscribeResponse SubscribeMiner(string signature = null, string sessionId = null)
        {
            var context = (StratumContext) JsonRpcContext.Current().Value;

            var response = new SubscribeResponse
            {
                ExtraNonce1 = context.Miner.ExtraNonce.ToString("x8"), // Hex-encoded, per-connection unique string which will be used for coinbase serialization later. (http://mining.bitcoin.cz/stratum-mining)
                ExtraNonce2Size = ExtraNonce.ExpectedExtraNonce2Size // Represents expected length of extranonce2 which will be generated by the miner. (http://mining.bitcoin.cz/stratum-mining)
            };

            context.Miner.Subscribe(signature);

            return response;
        }

        /// <summary>
        /// Authorise a miner based on their username and password
        /// </summary>
        /// <param name="user">Worker Username (e.g. "coinium.1").</param>
        /// <param name="password">Worker Password (e.g. "x").</param>
        [JsonRpcMethod("mining.authorize")]
        public bool AuthorizeMiner(string user, string password)
        {
            var context = (StratumContext)JsonRpcContext.Current().Value;
            return context.Miner.Authenticate(user, password);
        }

        /// <summary>
        /// Allows a miner to submit the work they have done
        /// </summary>
        /// <param name="user">Worker Username.</param>
        /// <param name="jobId">Job ID(Should be unique per Job to ensure that share diff is recorded properly) </param>
        /// <param name="extraNonce2">Hex-encoded big-endian extranonce2, length depends on extranonce2_size from mining.notify</param>
        /// <param name="nTime"> UNIX timestamp (32bit integer, big-endian, hex-encoded), must be >= ntime provided by mining,notify and <= current time'</param>
        /// <param name="nonce"> 32bit integer hex-encoded, big-endian </param>
        /// <param name="cycle"> comma separated cycle edges in hex </param>
        [JsonRpcMethod("mining.submit")]
        public bool SubmitWork(string user, string jobId, string extraNonce2, string nTime, string nonce, string cycle)
        {
            var context = (StratumContext)JsonRpcContext.Current().Value;
            return _shareManager.ProcessShare(context.Miner, jobId, extraNonce2, nTime, nonce, cycle).IsValid;
        }
    }
}
