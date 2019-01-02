using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HexiPicture.Models
{
    public class PeriodsInfo
    {
        /// <summary>
        /// ID
        /// </summary>
        public int? id { get; set; }
        /// <summary>
        /// 期数
        /// </summary>
        public string periods { get; set; }
        /// <summary>
        /// 主题
        /// </summary>
        public string theme { get; set; }
        /// <summary>
        /// 主题描述
        /// </summary>
        public string themeContent { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public string startTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public string endTime { get; set; }
        /// <summary>
        /// 主办单位
        /// </summary>
        public string sponsor { get; set; }
        /// <summary>
        /// 参赛规则
        /// </summary>
        public string roles { get; set; }
        /// <summary>
        /// 评奖方式
        /// </summary>
        public string appraiseWay { get; set; }
        /// <summary>
        /// 注意事项
        /// </summary>
        public string mattersNeedAttention { get; set; }
        /// <summary>
        /// 奖项设置
        /// </summary>
        public Awards[] awardSetting { get; set; }

    }

    public class Awards
    {
        /// <summary>
        /// 奖项
        /// </summary>
        public string award { get; set; }
        /// <summary>
        /// 奖品
        /// </summary>
        public string prize { get; set; }
        /// <summary>
        /// 人数
        /// </summary>
        public int? number { get; set; }
    }
}