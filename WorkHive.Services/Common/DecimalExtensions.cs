using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkHive.Services.Common;

using System.Globalization;

public static class DecimalExtensions
{
    /// <summary>
    /// Format số tiền thành chuỗi VNĐ (₫) với tùy chọn hiển thị phần thập phân.
    /// </summary>
    /// <param name="value">Giá trị tiền (decimal)</param>
    /// <param name="alwaysShowDecimals">Luôn hiển thị 3 số thập phân (mặc định: false)</param>
    /// <param name="useThousandSeparator">Sử dụng dấu phân cách hàng nghìn (mặc định: true)</param>
    /// <returns>Chuỗi đã format (ví dụ: "250.560₫" hoặc "1.000.000₫")</returns>
    //public static string ToVnd(this decimal? value, bool alwaysShowDecimals = false, bool useThousandSeparator = true)
    //{
    //    if (!value.HasValue) return "0₫"; // hoặc trả về string.Empty tùy nhu cầu

    //    return value.Value.ToVnd(alwaysShowDecimals, useThousandSeparator);
    //}

    //public static string ToVnd(this decimal value, bool alwaysShowDecimals = false, bool useThousandSeparator = true)
    //{
    //    // Giữ nguyên phần implement gốc của bạn ở đây
    //    bool hasNoDecimals = value == Math.Floor(value);
    //    string format = alwaysShowDecimals || !hasNoDecimals ? "#,##0.000₫" : "#,##0₫";

    //    if (!useThousandSeparator)
    //    {
    //        format = format.Replace("#,", "#");
    //    }

    //    return value.ToString(format, new CultureInfo("vi-VN"))
    //             .Replace(",", "~")
    //             .Replace(".", ",")
    //             .Replace("~", ".");
    //}

    public static string ToVnd(this decimal? value, bool alwaysShowDecimals = false, bool useThousandSeparator = true)
    {
        if (!value.HasValue) return "0₫";
        return value.Value.ToVnd(alwaysShowDecimals, useThousandSeparator);
    }

    public static string ToVnd(this decimal value, bool alwaysShowDecimals = false, bool useThousandSeparator = true)
    {
        bool hasNoDecimals = value == Math.Floor(value);
        string format = alwaysShowDecimals || !hasNoDecimals ? "#,##0.000" : "#,##0";

        // Tạo format số với CultureInfo tùy chỉnh
        var culture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
        culture.NumberFormat.NumberGroupSeparator = ".";  // Dấu chấm phân cách hàng nghìn
        culture.NumberFormat.NumberDecimalSeparator = ","; // Dấu phẩy phân cách thập phân

        string formattedNumber = value.ToString(format, culture);

        // Bỏ dấu phân cách hàng nghìn nếu không dùng
        if (!useThousandSeparator)
        {
            formattedNumber = formattedNumber.Replace(".", "");
        }

        return $"{formattedNumber}₫";
    }
}
