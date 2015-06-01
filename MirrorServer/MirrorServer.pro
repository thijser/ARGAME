TEMPLATE = subdirs

SUBDIRS = Server \
          UnitTests

UnitTests.depends = Server
