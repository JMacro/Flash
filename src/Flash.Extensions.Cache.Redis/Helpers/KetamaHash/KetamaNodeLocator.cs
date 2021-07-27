﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flash.Extensions.Cache.Redis.Helpers.KetamaHash
{
    /// <summary>
    /// 一致性哈希
    /// </summary>
    public class KetamaNodeLocator
    {
        //原文中的JAVA类TreeMap实现了Comparator方法，这里我图省事，直接用了net下的SortedList，其中Comparer接口方法）
        private SortedList<long, string> ketamaNodes = new SortedList<long, string>();
        private int numReps = 160;

        /// <summary>
        /// 此处参数与JAVA版中有区别，因为使用的静态方法，所以不再传递HashAlgorithm alg参数
        /// </summary>
        /// <param name="nodes">服务器节点</param>
        /// <param name="nodeCopies">虚拟节点数</param>
        public KetamaNodeLocator(List<string> nodes, int nodeCopies)
        {
            ketamaNodes = new SortedList<long, string>();

            numReps = nodeCopies;
            //对所有节点，生成nCopies个虚拟结点
            foreach (string node in nodes)
            {
                //每四个虚拟结点为一组
                for (int i = 0; i < numReps / 4; i++)
                {
                    //getKeyForNode方法为这组虚拟结点得到惟一名称 
                    byte[] digest = HashAlgorithm.computeMd5(node + i);
                    /** Md5是一个16字节长度的数组，将16字节的数组每四个字节一组，分别对应一个虚拟结点，这就是为什么上面把虚拟结点四个划分一组的原因*/
                    for (int h = 0; h < 4; h++)
                    {
                        long m = HashAlgorithm.hash(digest, h);
                        ketamaNodes[m] = node;
                    }
                }
            }
        }

        public string GetPrimary(string k)
        {
            byte[] digest = HashAlgorithm.computeMd5(k);
            string rv = GetNodeForKey(HashAlgorithm.hash(digest, 0));
            return rv;
        }

        string GetNodeForKey(long hash)
        {
            string rv;
            long key = hash;
            //如果找到这个节点，直接取节点，返回   
            if (!ketamaNodes.ContainsKey(key))
            {
                //得到大于当前key的那个子Map，然后从中取出第一个key，就是大于且离它最近的那个key 说明详见: http://www.javaeye.com/topic/684087
                var tailMap = from coll in ketamaNodes
                              where coll.Key > hash
                              select new { coll.Key };
                if (tailMap == null || tailMap.Count() == 0)
                    key = ketamaNodes.FirstOrDefault().Key;
                else
                    key = tailMap.FirstOrDefault().Key;
            }
            rv = ketamaNodes[key];
            return rv;
        }
    }
}
