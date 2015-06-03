#include <gtest/gtest.h>
#include "averager.hpp"

using namespace mirrors;

TEST(AveragerTest, SingleValue) {
    Averager<float> averager(1);
    ASSERT_EQ(averager.update(3.0f), 3.0f);
}

TEST(AveragerTest, TwoValues) {
    Averager<float> averager(2);
    ASSERT_EQ(averager.update(1.0f), 1.0f);
    ASSERT_EQ(averager.update(2.0f), 1.5f);
}

TEST(AveragerTest, HistoryLength) {
    Averager<float> averager(2);
    ASSERT_EQ(averager.update(1.0f), 1.0f);
    ASSERT_EQ(averager.update(2.0f), 1.5f);
    ASSERT_EQ(averager.update(3.0f), 2.5f);
}

TEST(AveragerTest, Flush) {
    Averager<float> averager(2, 5.0f);
    ASSERT_EQ(averager.update(1.0f), 1.0f);
    ASSERT_EQ(averager.update(2.0f), 1.5f);
    ASSERT_EQ(averager.update(20.0f), 20.0f);
}

TEST(AveragerTest, SinglePoint) {
    Averager<Point> averager(1);
    ASSERT_EQ(averager.update(Point(1, 1)), Point(1, 1));
}

TEST(AveragerTest, TwoPoints) {
    Averager<Point> averager(2);
    ASSERT_EQ(averager.update(Point(1, 1)), Point(1, 1));
    ASSERT_EQ(averager.update(Point(3, 9)), Point(2, 5));
}

TEST(AveragerTest, FlushPoints) {
    Averager<Point> averager(2, 5.0f);
    ASSERT_EQ(averager.update(Point(1, 1)), Point(1, 1));
    ASSERT_EQ(averager.update(Point(3, 9)), Point(2, 5));
    ASSERT_EQ(averager.update(Point(20, 9)), Point(20, 9));
}
