﻿/************************************************************************************
 * Copyright (c) 2017 All Rights Reserved. 
 * CLR版本：4.0.30319.42000
 * 机器名称：JIAN
 * 命名空间：JCE.Utils.Common
 * 文件名：GuidUtil
 * 版本号：v1.0.0.0
 * 唯一标识：b02807e2-2f3c-4a2a-a805-b706f989cd1e
 * 当前的用户域：JIAN
 * 创建人：简玄冰
 * 电子邮箱：jianxuanhuo1@126.com
 * 创建时间：2017/2/12 23:16:36
 * 描述：
 *
 * =====================================================================
 * 修改标记：
 * 修改时间：2017/2/12 23:16:36
 * 修改人：简玄冰
 * 版本号：v1.0.0.0
 * 描述：
 *
/************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCE.Utils.Common
{
    /// <summary>
    /// Guid工具类
    /// </summary>
    public class GuidUtil
    {
        /// <summary>
        /// 1970年1月1日毫秒
        /// </summary>
        private static readonly long EpochMilliseconds = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks /
                                                         10000L;
        #region NewGuid(获取Guid)
        /// <summary>
        /// 获取Guid，根据SqlServer排序规则顺序创建Guid
        /// </summary>
        /// <returns></returns>
        public static Guid NewSequentialGuid()
        {
            //此处GUID 不能在插入多个主机时保证唯一性，但在单个主机时，可以保证唯一性
            var guidBytes = Guid.NewGuid().ToByteArray();
            byte[] sequential = BitConverter.GetBytes(DateTime.Now.Ticks / 10000L - EpochMilliseconds);
            //丢弃2个最大字节数，并不是问题
            if (BitConverter.IsLittleEndian)
            {
                guidBytes[10] = sequential[5];
                guidBytes[11] = sequential[4];
                guidBytes[12] = sequential[3];
                guidBytes[13] = sequential[2];
                guidBytes[14] = sequential[1];
                guidBytes[15] = sequential[0];
            }
            else
            {
                Buffer.BlockCopy(sequential, 2, guidBytes, 10, 6);
            }
            return new Guid(guidBytes);
        }

        /// <summary>
        /// 获取Guid字符串，不带"-"分隔符
        /// </summary>
        /// <returns></returns>
        public static string NewStringGuidD()
        {
            Guid guid = NewSequentialGuid();
            return guid.ToString("D");
        }
        /// <summary>
        /// 获取Guid字符串，带"-"分隔符
        /// </summary>
        /// <returns></returns>
        public static string NewStringGuidN()
        {
            Guid guid = NewSequentialGuid();
            return guid.ToString("N");
        }
        #endregion

        #region GetGuidString(获取Guid字符串)
        /// <summary>
        /// 获取Guid字符串
        /// </summary>
        /// <returns></returns>
        public static string GetGuidString()
        {
            return Guid.NewGuid().ToString();
        }
        #endregion

        #region GetGuidLong(获取Guid长整型)
        /// <summary>
        /// 获取Guid长整型
        /// 后话：原来BitConverter.ToInt64方法，只取buffer从startIndex开始向后加7个字节的值。
        /// 也就是说，我们16字节的高8个字节被忽略掉了。GUID理想情况下，要2^128个数据才会出现冲突，
        /// 而转换后，把字节数减半，也就是2^64数据就会出现冲突。
        /// </summary>
        /// <returns></returns>
        public static long GetGuidLong()
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();
            return BitConverter.ToInt64(buffer, 0);
        }
        #endregion
    }
}
