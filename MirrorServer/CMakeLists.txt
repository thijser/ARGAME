project(MirrorServer)
cmake_minimum_required(VERSION 2.8)
aux_source_directory(. SRC_LIST)
aux_source_directory(netlink NETLINK_SOCKETS)
add_executable(${PROJECT_NAME} ${SRC_LIST} ${NETLINK_SOCKETS})

