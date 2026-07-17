-- --------------------------------------------------------
-- Host:                         10.0.13.219
-- Wersja serwera:               8.0.42-0ubuntu0.24.04.1 - (Ubuntu)
-- Serwer OS:                    Linux
-- HeidiSQL Wersja:              12.10.0.7000
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;


-- Zrzut struktury bazy danych eye_db
CREATE DATABASE IF NOT EXISTS `eye_db` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_polish_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `eye_db`;

-- Zrzut struktury tabela eye_db.auth_group
CREATE TABLE IF NOT EXISTS `auth_group` (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(150) COLLATE utf8mb4_polish_ci NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `name` (`name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_polish_ci;

-- Zrzucanie danych dla tabeli eye_db.auth_group: ~0 rows (około)

-- Zrzut struktury tabela eye_db.auth_group_permissions
CREATE TABLE IF NOT EXISTS `auth_group_permissions` (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `group_id` int NOT NULL,
  `permission_id` int NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `auth_group_permissions_group_id_permission_id_0cd325b0_uniq` (`group_id`,`permission_id`),
  KEY `auth_group_permissio_permission_id_84c5c92e_fk_auth_perm` (`permission_id`),
  CONSTRAINT `auth_group_permissio_permission_id_84c5c92e_fk_auth_perm` FOREIGN KEY (`permission_id`) REFERENCES `auth_permission` (`id`),
  CONSTRAINT `auth_group_permissions_group_id_b120cbf9_fk_auth_group_id` FOREIGN KEY (`group_id`) REFERENCES `auth_group` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_polish_ci;

-- Zrzucanie danych dla tabeli eye_db.auth_group_permissions: ~0 rows (około)

-- Zrzut struktury tabela eye_db.auth_permission
CREATE TABLE IF NOT EXISTS `auth_permission` (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(255) COLLATE utf8mb4_polish_ci NOT NULL,
  `content_type_id` int NOT NULL,
  `codename` varchar(100) COLLATE utf8mb4_polish_ci NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `auth_permission_content_type_id_codename_01ab375a_uniq` (`content_type_id`,`codename`),
  CONSTRAINT `auth_permission_content_type_id_2f476e4b_fk_django_co` FOREIGN KEY (`content_type_id`) REFERENCES `django_content_type` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=73 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_polish_ci;

-- Zrzucanie danych dla tabeli eye_db.auth_permission: ~72 rows (około)
INSERT INTO `auth_permission` (`id`, `name`, `content_type_id`, `codename`) VALUES
	(1, 'Can add log entry', 1, 'add_logentry'),
	(2, 'Can change log entry', 1, 'change_logentry'),
	(3, 'Can delete log entry', 1, 'delete_logentry'),
	(4, 'Can view log entry', 1, 'view_logentry'),
	(5, 'Can add permission', 2, 'add_permission'),
	(6, 'Can change permission', 2, 'change_permission'),
	(7, 'Can delete permission', 2, 'delete_permission'),
	(8, 'Can view permission', 2, 'view_permission'),
	(9, 'Can add group', 3, 'add_group'),
	(10, 'Can change group', 3, 'change_group'),
	(11, 'Can delete group', 3, 'delete_group'),
	(12, 'Can view group', 3, 'view_group'),
	(13, 'Can add user', 4, 'add_user'),
	(14, 'Can change user', 4, 'change_user'),
	(15, 'Can delete user', 4, 'delete_user'),
	(16, 'Can view user', 4, 'view_user'),
	(17, 'Can add content type', 5, 'add_contenttype'),
	(18, 'Can change content type', 5, 'change_contenttype'),
	(19, 'Can delete content type', 5, 'delete_contenttype'),
	(20, 'Can view content type', 5, 'view_contenttype'),
	(21, 'Can add session', 6, 'add_session'),
	(22, 'Can change session', 6, 'change_session'),
	(23, 'Can delete session', 6, 'delete_session'),
	(24, 'Can view session', 6, 'view_session'),
	(25, 'Can add dht11', 7, 'add_dht11'),
	(26, 'Can change dht11', 7, 'change_dht11'),
	(27, 'Can delete dht11', 7, 'delete_dht11'),
	(28, 'Can view dht11', 7, 'view_dht11'),
	(29, 'Can add ens160', 8, 'add_ens160'),
	(30, 'Can change ens160', 8, 'change_ens160'),
	(31, 'Can delete ens160', 8, 'delete_ens160'),
	(32, 'Can view ens160', 8, 'view_ens160'),
	(33, 'Can add gy906', 9, 'add_gy906'),
	(34, 'Can change gy906', 9, 'change_gy906'),
	(35, 'Can delete gy906', 9, 'delete_gy906'),
	(36, 'Can view gy906', 9, 'view_gy906'),
	(37, 'Can add room', 10, 'add_room'),
	(38, 'Can change room', 10, 'change_room'),
	(39, 'Can delete room', 10, 'delete_room'),
	(40, 'Can view room', 10, 'view_room'),
	(41, 'Can add sen22396', 11, 'add_sen22396'),
	(42, 'Can change sen22396', 11, 'change_sen22396'),
	(43, 'Can delete sen22396', 11, 'delete_sen22396'),
	(44, 'Can view sen22396', 11, 'view_sen22396'),
	(45, 'Can add access', 12, 'add_access'),
	(46, 'Can change access', 12, 'change_access'),
	(47, 'Can delete access', 12, 'delete_access'),
	(48, 'Can view access', 12, 'view_access'),
	(49, 'Can add room actions', 13, 'add_roomactions'),
	(50, 'Can change room actions', 13, 'change_roomactions'),
	(51, 'Can delete room actions', 13, 'delete_roomactions'),
	(52, 'Can view room actions', 13, 'view_roomactions'),
	(53, 'Can add room qualities', 14, 'add_roomqualities'),
	(54, 'Can change room qualities', 14, 'change_roomqualities'),
	(55, 'Can delete room qualities', 14, 'delete_roomqualities'),
	(56, 'Can view room qualities', 14, 'view_roomqualities'),
	(57, 'Can add sensor device', 15, 'add_sensordevice'),
	(58, 'Can change sensor device', 15, 'change_sensordevice'),
	(59, 'Can delete sensor device', 15, 'delete_sensordevice'),
	(60, 'Can view sensor device', 15, 'view_sensordevice'),
	(61, 'Can add measurement', 16, 'add_measurement'),
	(62, 'Can change measurement', 16, 'change_measurement'),
	(63, 'Can delete measurement', 16, 'delete_measurement'),
	(64, 'Can view measurement', 16, 'view_measurement'),
	(65, 'Can add parameter visibility', 17, 'add_parametervisibility'),
	(66, 'Can change parameter visibility', 17, 'change_parametervisibility'),
	(67, 'Can delete parameter visibility', 17, 'delete_parametervisibility'),
	(68, 'Can view parameter visibility', 17, 'view_parametervisibility'),
	(69, 'Can add parameter range', 18, 'add_parameterrange'),
	(70, 'Can change parameter range', 18, 'change_parameterrange'),
	(71, 'Can delete parameter range', 18, 'delete_parameterrange'),
	(72, 'Can view parameter range', 18, 'view_parameterrange');

-- Zrzut struktury tabela eye_db.auth_user
CREATE TABLE IF NOT EXISTS `auth_user` (
  `id` int NOT NULL AUTO_INCREMENT,
  `password` varchar(128) COLLATE utf8mb4_polish_ci NOT NULL,
  `last_login` datetime(6) DEFAULT NULL,
  `is_superuser` tinyint(1) NOT NULL,
  `username` varchar(150) COLLATE utf8mb4_polish_ci NOT NULL,
  `first_name` varchar(150) COLLATE utf8mb4_polish_ci NOT NULL,
  `last_name` varchar(150) COLLATE utf8mb4_polish_ci NOT NULL,
  `email` varchar(254) COLLATE utf8mb4_polish_ci NOT NULL,
  `is_staff` tinyint(1) NOT NULL,
  `is_active` tinyint(1) NOT NULL,
  `date_joined` datetime(6) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `username` (`username`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_polish_ci;

-- Zrzucanie danych dla tabeli eye_db.auth_user: ~1 rows (około)
INSERT INTO `auth_user` (`id`, `password`, `last_login`, `is_superuser`, `username`, `first_name`, `last_name`, `email`, `is_staff`, `is_active`, `date_joined`) VALUES
	(1, 'pbkdf2_sha256$1000000$zsFBgCz03iHVZASsXAadQz$nNwBIbsKaZnt1Xm4FBd4iUHyYoaUzURRmyBiFguFu9I=', '2025-05-31 11:06:40.621012', 1, 'admin', '', '', 'admin@admin.admin', 1, 1, '2025-05-31 11:05:12.657533');

-- Zrzut struktury tabela eye_db.auth_user_groups
CREATE TABLE IF NOT EXISTS `auth_user_groups` (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `user_id` int NOT NULL,
  `group_id` int NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `auth_user_groups_user_id_group_id_94350c0c_uniq` (`user_id`,`group_id`),
  KEY `auth_user_groups_group_id_97559544_fk_auth_group_id` (`group_id`),
  CONSTRAINT `auth_user_groups_group_id_97559544_fk_auth_group_id` FOREIGN KEY (`group_id`) REFERENCES `auth_group` (`id`),
  CONSTRAINT `auth_user_groups_user_id_6a12ed8b_fk_auth_user_id` FOREIGN KEY (`user_id`) REFERENCES `auth_user` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_polish_ci;

-- Zrzucanie danych dla tabeli eye_db.auth_user_groups: ~0 rows (około)

-- Zrzut struktury tabela eye_db.auth_user_user_permissions
CREATE TABLE IF NOT EXISTS `auth_user_user_permissions` (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `user_id` int NOT NULL,
  `permission_id` int NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `auth_user_user_permissions_user_id_permission_id_14a6b632_uniq` (`user_id`,`permission_id`),
  KEY `auth_user_user_permi_permission_id_1fbb5f2c_fk_auth_perm` (`permission_id`),
  CONSTRAINT `auth_user_user_permi_permission_id_1fbb5f2c_fk_auth_perm` FOREIGN KEY (`permission_id`) REFERENCES `auth_permission` (`id`),
  CONSTRAINT `auth_user_user_permissions_user_id_a95ead1b_fk_auth_user_id` FOREIGN KEY (`user_id`) REFERENCES `auth_user` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_polish_ci;

-- Zrzucanie danych dla tabeli eye_db.auth_user_user_permissions: ~0 rows (około)

-- Zrzut struktury tabela eye_db.django_admin_log
CREATE TABLE IF NOT EXISTS `django_admin_log` (
  `id` int NOT NULL AUTO_INCREMENT,
  `action_time` datetime(6) NOT NULL,
  `object_id` longtext COLLATE utf8mb4_polish_ci,
  `object_repr` varchar(200) COLLATE utf8mb4_polish_ci NOT NULL,
  `action_flag` smallint unsigned NOT NULL,
  `change_message` longtext COLLATE utf8mb4_polish_ci NOT NULL,
  `content_type_id` int DEFAULT NULL,
  `user_id` int NOT NULL,
  PRIMARY KEY (`id`),
  KEY `django_admin_log_content_type_id_c4bce8eb_fk_django_co` (`content_type_id`),
  KEY `django_admin_log_user_id_c564eba6_fk_auth_user_id` (`user_id`),
  CONSTRAINT `django_admin_log_content_type_id_c4bce8eb_fk_django_co` FOREIGN KEY (`content_type_id`) REFERENCES `django_content_type` (`id`),
  CONSTRAINT `django_admin_log_user_id_c564eba6_fk_auth_user_id` FOREIGN KEY (`user_id`) REFERENCES `auth_user` (`id`),
  CONSTRAINT `django_admin_log_chk_1` CHECK ((`action_flag` >= 0))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_polish_ci;

-- Zrzucanie danych dla tabeli eye_db.django_admin_log: ~0 rows (około)

-- Zrzut struktury tabela eye_db.django_content_type
CREATE TABLE IF NOT EXISTS `django_content_type` (
  `id` int NOT NULL AUTO_INCREMENT,
  `app_label` varchar(100) COLLATE utf8mb4_polish_ci NOT NULL,
  `model` varchar(100) COLLATE utf8mb4_polish_ci NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `django_content_type_app_label_model_76bd3d3b_uniq` (`app_label`,`model`)
) ENGINE=InnoDB AUTO_INCREMENT=19 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_polish_ci;

-- Zrzucanie danych dla tabeli eye_db.django_content_type: ~18 rows (około)
INSERT INTO `django_content_type` (`id`, `app_label`, `model`) VALUES
	(1, 'admin', 'logentry'),
	(3, 'auth', 'group'),
	(2, 'auth', 'permission'),
	(4, 'auth', 'user'),
	(5, 'contenttypes', 'contenttype'),
	(12, 'eye_app', 'access'),
	(7, 'eye_app', 'dht11'),
	(8, 'eye_app', 'ens160'),
	(9, 'eye_app', 'gy906'),
	(16, 'eye_app', 'measurement'),
	(18, 'eye_app', 'parameterrange'),
	(17, 'eye_app', 'parametervisibility'),
	(10, 'eye_app', 'room'),
	(13, 'eye_app', 'roomactions'),
	(14, 'eye_app', 'roomqualities'),
	(11, 'eye_app', 'sen22396'),
	(15, 'eye_app', 'sensordevice'),
	(6, 'sessions', 'session');

-- Zrzut struktury tabela eye_db.django_migrations
CREATE TABLE IF NOT EXISTS `django_migrations` (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `app` varchar(255) COLLATE utf8mb4_polish_ci NOT NULL,
  `name` varchar(255) COLLATE utf8mb4_polish_ci NOT NULL,
  `applied` datetime(6) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=22 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_polish_ci;

-- Zrzucanie danych dla tabeli eye_db.django_migrations: ~20 rows (około)
INSERT INTO `django_migrations` (`id`, `app`, `name`, `applied`) VALUES
	(1, 'contenttypes', '0001_initial', '2025-05-31 10:58:49.419181'),
	(2, 'auth', '0001_initial', '2025-05-31 10:59:11.689123'),
	(3, 'admin', '0001_initial', '2025-05-31 10:59:16.513598'),
	(4, 'admin', '0002_logentry_remove_auto_add', '2025-05-31 10:59:17.202173'),
	(5, 'admin', '0003_logentry_add_action_flag_choices', '2025-05-31 10:59:17.263095'),
	(6, 'contenttypes', '0002_remove_content_type_name', '2025-05-31 10:59:20.788021'),
	(7, 'auth', '0002_alter_permission_name_max_length', '2025-05-31 10:59:23.490960'),
	(8, 'auth', '0003_alter_user_email_max_length', '2025-05-31 10:59:24.266400'),
	(9, 'auth', '0004_alter_user_username_opts', '2025-05-31 10:59:24.354677'),
	(10, 'auth', '0005_alter_user_last_login_null', '2025-05-31 10:59:25.564970'),
	(11, 'auth', '0006_require_contenttypes_0002', '2025-05-31 10:59:26.218588'),
	(12, 'auth', '0007_alter_validators_add_error_messages', '2025-05-31 10:59:34.649364'),
	(13, 'auth', '0008_alter_user_username_max_length', '2025-05-31 11:01:07.602935'),
	(14, 'auth', '0009_alter_user_last_name_max_length', '2025-05-31 11:01:08.977252'),
	(15, 'auth', '0010_alter_group_name_max_length', '2025-05-31 11:01:09.746907'),
	(16, 'auth', '0011_update_proxy_permissions', '2025-05-31 11:01:09.810146'),
	(17, 'auth', '0012_alter_user_first_name_max_length', '2025-05-31 11:01:11.770886'),
	(18, 'eye_app', '0001_initial', '2025-05-31 11:04:35.963322'),
	(19, 'sessions', '0001_initial', '2025-05-31 11:04:37.635077'),
	(20, 'eye_app', '0002_alter_measurement_timestamp', '2025-05-31 11:16:56.358096'),
	(21, 'eye_app', '0003_remove_roomactions_port', '2025-05-31 10:54:05.132386');

-- Zrzut struktury tabela eye_db.django_session
CREATE TABLE IF NOT EXISTS `django_session` (
  `session_key` varchar(40) COLLATE utf8mb4_polish_ci NOT NULL,
  `session_data` longtext COLLATE utf8mb4_polish_ci NOT NULL,
  `expire_date` datetime(6) NOT NULL,
  PRIMARY KEY (`session_key`),
  KEY `django_session_expire_date_a5c62663` (`expire_date`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_polish_ci;

-- Zrzucanie danych dla tabeli eye_db.django_session: ~2 rows (około)
INSERT INTO `django_session` (`session_key`, `session_data`, `expire_date`) VALUES
	('knhqp1yeqzv3h7ymu5whykrlr1r498ys', '.eJxVjMEOwiAQRP-FsyFsSwt49N5vILuwSNVAUtqT8d9tkx70OPPezFt43Nbst8aLn6O4ChCX344wPLkcID6w3KsMtazLTPJQ5EmbnGrk1-10_w4ytryvlcMBe3CIiUfSzjomCAo1u0TJJFADgjbW9hS7Lozc7xGUIzKoCLT4fAH1UDga:1uLIAZ:B-z9LN6fpjhM86-aD9QaDLBvQWzN2JqXYwbpp2vTtGM', '2025-06-14 11:06:23.356426'),
	('wr59os2jo4c138jm1rfa92o5ktjtanj4', '.eJxVjMEOwiAQRP-FsyFsSwt49N5vILuwSNVAUtqT8d9tkx70OPPezFt43Nbst8aLn6O4ChCX344wPLkcID6w3KsMtazLTPJQ5EmbnGrk1-10_w4ytryvlcMBe3CIiUfSzjomCAo1u0TJJFADgjbW9hS7Lozc7xGUIzKoCLT4fAH1UDga:1uLIAv:X0jyGC2gmFQI7smOcuKP37TG4mqZ1PqSIFHvuoTPlAE', '2025-06-14 11:06:45.526638');

-- Zrzut struktury tabela eye_db.eye_app_access
CREATE TABLE IF NOT EXISTS `eye_app_access` (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `has_read_access` tinyint(1) NOT NULL,
  `has_read_write_access` tinyint(1) NOT NULL,
  `user_id` int NOT NULL,
  `room_id` bigint NOT NULL,
  PRIMARY KEY (`id`),
  KEY `eye_app_access_user_id_a18217bd_fk_auth_user_id` (`user_id`),
  KEY `eye_app_access_room_id_5f8b0adb_fk_eye_app_room_id` (`room_id`),
  CONSTRAINT `eye_app_access_room_id_5f8b0adb_fk_eye_app_room_id` FOREIGN KEY (`room_id`) REFERENCES `eye_app_room` (`id`),
  CONSTRAINT `eye_app_access_user_id_a18217bd_fk_auth_user_id` FOREIGN KEY (`user_id`) REFERENCES `auth_user` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_polish_ci;

-- Zrzucanie danych dla tabeli eye_db.eye_app_access: ~0 rows (około)

-- Zrzut struktury tabela eye_db.eye_app_dht11
CREATE TABLE IF NOT EXISTS `eye_app_dht11` (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `humidity` double NOT NULL,
  `temperature` double NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_polish_ci;

-- Zrzucanie danych dla tabeli eye_db.eye_app_dht11: ~0 rows (około)

-- Zrzut struktury tabela eye_db.eye_app_ens160
CREATE TABLE IF NOT EXISTS `eye_app_ens160` (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `tvoc` double NOT NULL,
  `eco2` double NOT NULL,
  `aqi` double NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=78 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_polish_ci;

-- Zrzucanie danych dla tabeli eye_db.eye_app_ens160: ~24 rows (około)
INSERT INTO `eye_app_ens160` (`id`, `tvoc`, `eco2`, `aqi`) VALUES
	(1, 179, 666, 2),
	(2, 177, 664, 2),
	(3, 196, 688, 2),
	(4, 191, 681, 2),
	(5, 195, 687, 2),
	(6, 214, 710, 2),
	(7, 195, 687, 2),
	(8, 195, 687, 2),
	(9, 198, 691, 2),
	(10, 202, 696, 2),
	(11, 189, 679, 2),
	(12, 188, 678, 2),
	(13, 181, 669, 2),
	(14, 199, 692, 2),
	(15, 204, 698, 2),
	(16, 204, 698, 2),
	(17, 221, 718, 3),
	(18, 230, 729, 3),
	(19, 247, 749, 3),
	(20, 263, 765, 3),
	(21, 246, 747, 3),
	(22, 233, 733, 3),
	(23, 226, 724, 3),
	(24, 241, 741, 3),
	(25, 212, 708, 2),
	(26, 224, 722, 3),
	(27, 219, 716, 2),
	(28, 204, 698, 2),
	(29, 204, 698, 2),
	(30, 201, 694, 2),
	(31, 205, 700, 2),
	(32, 200, 694, 2),
	(33, 178, 665, 2),
	(34, 165, 648, 2),
	(35, 177, 663, 2),
	(36, 158, 638, 2),
	(37, 158, 638, 2),
	(38, 164, 647, 2),
	(39, 158, 638, 2),
	(40, 159, 640, 2),
	(41, 162, 644, 2),
	(42, 158, 638, 2),
	(43, 167, 650, 2),
	(44, 159, 640, 2),
	(45, 157, 637, 2),
	(46, 158, 638, 2),
	(47, 142, 616, 2),
	(48, 158, 638, 2),
	(49, 160, 640, 2),
	(50, 151, 628, 2),
	(51, 146, 621, 2),
	(52, 151, 628, 2),
	(53, 144, 619, 2),
	(54, 158, 638, 2),
	(55, 141, 614, 2),
	(56, 146, 621, 2),
	(57, 152, 630, 2),
	(58, 141, 614, 2),
	(59, 141, 614, 2),
	(60, 144, 618, 2),
	(61, 158, 638, 2),
	(62, 142, 615, 2),
	(63, 143, 616, 2),
	(64, 158, 638, 2),
	(65, 140, 613, 2),
	(66, 139, 611, 2),
	(67, 136, 606, 2),
	(68, 127, 593, 2),
	(69, 126, 592, 2),
	(70, 115, 574, 2),
	(71, 117, 577, 2),
	(72, 113, 571, 2),
	(73, 136, 606, 2),
	(74, 120, 582, 2),
	(75, 132, 600, 2),
	(76, 117, 578, 2),
	(77, 116, 575, 2);

-- Zrzut struktury tabela eye_db.eye_app_gy906
CREATE TABLE IF NOT EXISTS `eye_app_gy906` (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `temperature` double NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=76 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_polish_ci;

-- Zrzucanie danych dla tabeli eye_db.eye_app_gy906: ~24 rows (około)
INSERT INTO `eye_app_gy906` (`id`, `temperature`) VALUES
	(1, 20.81),
	(2, 20.89),
	(3, 20.87),
	(4, 20.87),
	(5, 21.29),
	(6, 20.89),
	(7, 21.29),
	(8, 21.05),
	(9, 20.99),
	(10, 21.01),
	(11, 20.93),
	(12, 20.99),
	(13, 20.93),
	(14, 20.89),
	(15, 20.87),
	(16, 20.89),
	(17, 20.83),
	(18, 21.01),
	(19, 20.89),
	(20, 21.01),
	(21, 20.99),
	(22, 20.93),
	(23, 20.87),
	(24, 20.93),
	(25, 20.89),
	(26, 20.93),
	(27, 20.93),
	(28, 20.87),
	(29, 20.99),
	(30, 20.89),
	(31, 20.95),
	(32, 20.89),
	(33, 20.87),
	(34, 20.83),
	(35, 20.95),
	(36, 20.95),
	(37, 20.89),
	(38, 20.95),
	(39, 20.89),
	(40, 20.87),
	(41, 20.95),
	(42, 21.01),
	(43, 20.93),
	(44, 20.95),
	(45, 20.87),
	(46, 20.95),
	(47, 20.87),
	(48, 20.89),
	(49, 20.89),
	(50, 20.95),
	(51, 20.89),
	(52, 20.95),
	(53, 20.93),
	(54, 20.99),
	(55, 20.87),
	(56, 20.99),
	(57, 20.99),
	(58, 20.95),
	(59, 20.95),
	(60, 20.87),
	(61, 21.11),
	(62, 20.95),
	(63, 20.95),
	(64, 21.01),
	(65, 21.11),
	(66, 20.93),
	(67, 20.89),
	(68, 21.01),
	(69, 21.07),
	(70, 20.99),
	(71, 21.15),
	(72, 21.11),
	(73, 21.15),
	(74, 21.01),
	(75, 21.01);

-- Zrzut struktury tabela eye_db.eye_app_measurement
CREATE TABLE IF NOT EXISTS `eye_app_measurement` (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `timestamp` datetime(6) NOT NULL,
  `dht11_id` bigint DEFAULT NULL,
  `ens160_id` bigint DEFAULT NULL,
  `gy906_id` bigint DEFAULT NULL,
  `sen22396_id` bigint DEFAULT NULL,
  `dev_id_id` bigint NOT NULL,
  PRIMARY KEY (`id`),
  KEY `eye_app_measurement_dht11_id_6dfa044d_fk_eye_app_dht11_id` (`dht11_id`),
  KEY `eye_app_measurement_ens160_id_959e293e_fk_eye_app_ens160_id` (`ens160_id`),
  KEY `eye_app_measurement_gy906_id_de51fc73_fk_eye_app_gy906_id` (`gy906_id`),
  KEY `eye_app_measurement_sen22396_id_70a08bf1_fk_eye_app_sen22396_id` (`sen22396_id`),
  KEY `eye_app_measurement_dev_id_id_4fcef8be_fk_eye_app_s` (`dev_id_id`),
  CONSTRAINT `eye_app_measurement_dev_id_id_4fcef8be_fk_eye_app_s` FOREIGN KEY (`dev_id_id`) REFERENCES `eye_app_sensordevice` (`id`),
  CONSTRAINT `eye_app_measurement_dht11_id_6dfa044d_fk_eye_app_dht11_id` FOREIGN KEY (`dht11_id`) REFERENCES `eye_app_dht11` (`id`),
  CONSTRAINT `eye_app_measurement_ens160_id_959e293e_fk_eye_app_ens160_id` FOREIGN KEY (`ens160_id`) REFERENCES `eye_app_ens160` (`id`),
  CONSTRAINT `eye_app_measurement_gy906_id_de51fc73_fk_eye_app_gy906_id` FOREIGN KEY (`gy906_id`) REFERENCES `eye_app_gy906` (`id`),
  CONSTRAINT `eye_app_measurement_sen22396_id_70a08bf1_fk_eye_app_sen22396_id` FOREIGN KEY (`sen22396_id`) REFERENCES `eye_app_sen22396` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=72 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_polish_ci;

-- Zrzucanie danych dla tabeli eye_db.eye_app_measurement: ~20 rows (około)
INSERT INTO `eye_app_measurement` (`id`, `timestamp`, `dht11_id`, `ens160_id`, `gy906_id`, `sen22396_id`, `dev_id_id`) VALUES
	(1, '2025-05-31 08:28:16.315000', NULL, 5, 5, 5, 1),
	(2, '2025-05-31 09:19:09.877000', NULL, 6, 6, 6, 1),
	(3, '2025-05-31 08:28:16.315000', NULL, 7, 7, 7, 1),
	(4, '2025-05-31 09:22:33.373000', NULL, 8, 8, 8, 1),
	(5, '2025-05-31 09:23:22.097000', NULL, 9, 9, 9, 1),
	(6, '2025-05-31 09:24:17.327000', NULL, 10, 10, 10, 1),
	(7, '2025-05-31 09:25:02.051000', NULL, 11, 11, 11, 1),
	(8, '2025-05-31 09:27:37.025000', NULL, 12, 12, 12, 1),
	(9, '2025-05-31 09:28:25.449000', NULL, 13, 13, 13, 1),
	(10, '2025-05-31 09:30:05.598000', NULL, 14, 14, 14, 1),
	(11, '2025-05-31 09:31:57.649000', NULL, 15, 15, 15, 1),
	(12, '2025-05-31 09:32:52.680000', NULL, 16, 16, 16, 1),
	(13, '2025-05-31 09:33:40.898000', NULL, 17, 17, 17, 1),
	(14, '2025-05-31 09:34:35.728000', NULL, 18, 18, 18, 1),
	(15, '2025-05-31 09:35:23.846000', NULL, 19, 19, 19, 1),
	(16, '2025-05-31 09:36:18.376000', NULL, 20, 20, 20, 1),
	(17, '2025-05-31 09:37:06.901000', NULL, 21, 21, 21, 1),
	(18, '2025-05-31 09:38:01.825000', NULL, 22, 22, 22, 1),
	(19, '2025-05-31 09:38:50.149000', NULL, 23, 23, 23, 1),
	(20, '2025-05-31 09:39:44.974000', NULL, 24, 24, 24, 1),
	(21, '2025-05-31 10:12:16.098000', NULL, 25, 25, 25, 1),
	(22, '2025-05-31 10:13:10.524000', NULL, 26, 26, 26, 1),
	(23, '2025-05-31 10:13:59.150000', NULL, 27, 27, 27, 1),
	(24, '2025-05-31 10:14:53.980000', NULL, 28, 28, 28, 1),
	(25, '2025-05-31 10:16:37.230000', NULL, 30, 29, 29, 1),
	(26, '2025-05-31 10:17:25.548000', NULL, 31, 30, 30, 1),
	(27, '2025-05-31 10:18:20.278000', NULL, 32, 31, 31, 1),
	(28, '2025-05-31 10:20:03.526000', NULL, 33, 32, 32, 1),
	(29, '2025-05-31 10:20:52.550000', NULL, 34, 33, 33, 1),
	(30, '2025-05-31 10:21:47.174000', NULL, 35, 34, 34, 1),
	(31, '2025-05-31 10:22:35.598000', NULL, 36, 35, 35, 1),
	(32, '2025-05-31 10:22:35.598000', NULL, 37, 36, 36, 1),
	(33, '2025-05-31 10:23:30.122000', NULL, 38, 37, 37, 1),
	(34, '2025-05-31 10:22:35.598000', NULL, 39, 38, 38, 1),
	(35, '2025-05-31 10:24:19.047000', NULL, 40, 39, 39, 1),
	(36, '2025-05-31 10:25:13.671000', NULL, 41, 40, 40, 1),
	(37, '2025-05-31 10:22:35.598000', NULL, 42, 41, 41, 1),
	(38, '2025-05-31 10:26:02.296000', NULL, 43, 42, 42, 1),
	(39, '2025-05-31 10:26:56.922000', NULL, 44, 43, 43, 1),
	(40, '2025-05-31 10:22:35.598000', NULL, 46, 44, 44, 1),
	(41, '2025-05-31 10:28:39.970000', NULL, 47, 45, 45, 1),
	(42, '2025-05-31 10:22:35.598000', NULL, 48, 46, 46, 1),
	(43, '2025-05-31 10:29:27.894000', NULL, 49, 47, 47, 1),
	(44, '2025-05-31 10:30:22.718000', NULL, 50, 48, 48, 1),
	(45, '2025-05-31 10:31:10.843000', NULL, 51, 49, 49, 1),
	(46, '2025-05-31 10:32:05.467000', NULL, 52, 50, 50, 1),
	(47, '2025-05-31 10:32:54.091000', NULL, 53, 51, 51, 1),
	(48, '2025-05-31 10:22:35.598000', NULL, 54, 52, 52, 1),
	(49, '2025-05-31 10:33:48.615000', NULL, 55, 53, 53, 1),
	(50, '2025-05-31 10:34:37.139000', NULL, 56, 54, 54, 1),
	(51, '2025-05-31 10:35:31.663000', NULL, 57, 55, 55, 1),
	(52, '2025-05-31 10:36:20.287000', NULL, 58, 56, 56, 1),
	(53, '2025-05-31 10:37:14.911000', NULL, 59, 57, 57, 1),
	(54, '2025-05-31 10:38:03.335000', NULL, 60, 58, 58, 1),
	(55, '2025-05-31 10:22:35.598000', NULL, 61, 59, 59, 1),
	(56, '2025-05-31 10:38:57.959000', NULL, 62, 60, 60, 1),
	(57, '2025-05-31 10:39:46.784000', NULL, 63, 61, 61, 1),
	(58, '2025-05-31 10:22:35.598000', NULL, 64, 62, 62, 1),
	(59, '2025-05-31 10:42:22.657000', NULL, 65, 63, 63, 1),
	(60, '2025-05-31 10:43:11.381000', NULL, 66, 64, 64, 1),
	(61, '2025-05-31 10:44:06.305000', NULL, 67, 65, 65, 1),
	(62, '2025-05-31 10:44:55.029000', NULL, 68, 66, 66, 1),
	(63, '2025-05-31 10:45:49.457000', NULL, 69, 67, 67, 1),
	(64, '2025-05-31 10:46:36.783000', NULL, 70, 68, 68, 1),
	(65, '2025-05-31 10:47:31.509000', NULL, 71, 69, 69, 1),
	(66, '2025-05-31 10:48:19.833000', NULL, 72, 70, 70, 1),
	(67, '2025-05-31 10:49:14.557000', NULL, 73, 71, 71, 1),
	(68, '2025-05-31 10:50:02.681000', NULL, 74, 72, 72, 1),
	(69, '2025-05-31 10:50:57.506000', NULL, 75, 73, 73, 1),
	(70, '2025-05-31 10:51:46.630000', NULL, 76, 74, 74, 1),
	(71, '2025-05-31 10:52:41.160000', NULL, 77, 75, 75, 1);

-- Zrzut struktury tabela eye_db.eye_app_parameterrange
CREATE TABLE IF NOT EXISTS `eye_app_parameterrange` (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `parameter` varchar(20) COLLATE utf8mb4_polish_ci NOT NULL,
  `quality` varchar(10) COLLATE utf8mb4_polish_ci NOT NULL,
  `min_value` double DEFAULT NULL,
  `max_value` double DEFAULT NULL,
  `room_id` bigint NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `eye_app_parameterrange_room_id_parameter_quality_602c574c_uniq` (`room_id`,`parameter`,`quality`),
  CONSTRAINT `eye_app_parameterrange_room_id_c343cb99_fk_eye_app_room_id` FOREIGN KEY (`room_id`) REFERENCES `eye_app_room` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_polish_ci;

-- Zrzucanie danych dla tabeli eye_db.eye_app_parameterrange: ~0 rows (około)

-- Zrzut struktury tabela eye_db.eye_app_parametervisibility
CREATE TABLE IF NOT EXISTS `eye_app_parametervisibility` (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `parameter` varchar(50) COLLATE utf8mb4_polish_ci NOT NULL,
  `visible` tinyint(1) NOT NULL,
  `room_id` bigint NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `eye_app_parametervisibility_room_id_parameter_f73adabd_uniq` (`room_id`,`parameter`),
  CONSTRAINT `eye_app_parametervisibility_room_id_1997126f_fk_eye_app_room_id` FOREIGN KEY (`room_id`) REFERENCES `eye_app_room` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_polish_ci;

-- Zrzucanie danych dla tabeli eye_db.eye_app_parametervisibility: ~7 rows (około)
INSERT INTO `eye_app_parametervisibility` (`id`, `parameter`, `visible`, `room_id`) VALUES
	(1, 'temperature_ambient', 1, 1),
	(2, 'temperature_close', 1, 1),
	(3, 'humidity', 1, 1),
	(4, 'co2', 1, 1),
	(5, 'tvoc', 1, 1),
	(6, 'eco2', 1, 1),
	(7, 'aqi', 1, 1);

-- Zrzut struktury tabela eye_db.eye_app_room
CREATE TABLE IF NOT EXISTS `eye_app_room` (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `room_number` varchar(10) COLLATE utf8mb4_polish_ci NOT NULL,
  `floor` int NOT NULL,
  `capacity` int NOT NULL,
  `description` varchar(100) COLLATE utf8mb4_polish_ci NOT NULL,
  `device_id_id` bigint DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `eye_app_room_device_id_id_0bf2e294_fk_eye_app_sensordevice_id` (`device_id_id`),
  CONSTRAINT `eye_app_room_device_id_id_0bf2e294_fk_eye_app_sensordevice_id` FOREIGN KEY (`device_id_id`) REFERENCES `eye_app_sensordevice` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_polish_ci;

-- Zrzucanie danych dla tabeli eye_db.eye_app_room: ~1 rows (około)
INSERT INTO `eye_app_room` (`id`, `room_number`, `floor`, `capacity`, `description`, `device_id_id`) VALUES
	(1, 'Salon', 2, 8, '10m2', 1);

-- Zrzut struktury tabela eye_db.eye_app_roomactions
CREATE TABLE IF NOT EXISTS `eye_app_roomactions` (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `parameter` varchar(50) COLLATE utf8mb4_polish_ci NOT NULL,
  `custom_name` varchar(100) COLLATE utf8mb4_polish_ci NOT NULL,
  `min` double NOT NULL,
  `max` double NOT NULL,
  `url` varchar(200) COLLATE utf8mb4_polish_ci NOT NULL,
  `room_id` bigint NOT NULL,
  PRIMARY KEY (`id`),
  KEY `eye_app_roomactions_room_id_08e4774c_fk_eye_app_room_id` (`room_id`),
  CONSTRAINT `eye_app_roomactions_room_id_08e4774c_fk_eye_app_room_id` FOREIGN KEY (`room_id`) REFERENCES `eye_app_room` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_polish_ci;

-- Zrzucanie danych dla tabeli eye_db.eye_app_roomactions: ~0 rows (około)
INSERT INTO `eye_app_roomactions` (`id`, `parameter`, `custom_name`, `min`, `max`, `url`, `room_id`) VALUES
	(1, 'temperature_ambient', '', 15, 25, 'http://10.0.13.231:8000/action', 1);

-- Zrzut struktury tabela eye_db.eye_app_roomqualities
CREATE TABLE IF NOT EXISTS `eye_app_roomqualities` (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `max_bad_temp_otocz` double NOT NULL,
  `min_mid_temp_otocz` double NOT NULL,
  `max_mid_temp_otocz` double NOT NULL,
  `min_good_temp_otocz` double NOT NULL,
  `max_good_temp_otocz` double NOT NULL,
  `min_mid2_temp_otocz` double NOT NULL,
  `max_mid2_temp_otocz` double NOT NULL,
  `min_bad2_temp_otocz` double NOT NULL,
  `max_bad_temp_zbliz` double NOT NULL,
  `min_mid_temp_zbliz` double NOT NULL,
  `max_mid_temp_zbliz` double NOT NULL,
  `min_good_temp_zbliz` double NOT NULL,
  `max_good_temp_zbliz` double NOT NULL,
  `min_mid2_temp_zbliz` double NOT NULL,
  `max_mid2_temp_zbliz` double NOT NULL,
  `min_bad2_temp_zbliz` double NOT NULL,
  `max_bad_wiglotnosc` double NOT NULL,
  `min_mid_wiglotnosc` double NOT NULL,
  `max_mid_wiglotnosc` double NOT NULL,
  `min_good_wiglotnosc` double NOT NULL,
  `max_good_wiglotnosc` double NOT NULL,
  `min_mid2_wiglotnosc` double NOT NULL,
  `max_mid2_wiglotnosc` double NOT NULL,
  `min_bad2_wiglotnosc` double NOT NULL,
  `max_good_co2` double NOT NULL,
  `min_mid_co2` double NOT NULL,
  `max_mid_co2` double NOT NULL,
  `min_bad_co2` double NOT NULL,
  `max_good_tvoc` double NOT NULL,
  `min_mid_tvoc` double NOT NULL,
  `max_mid_tvoc` double NOT NULL,
  `min_bad_tvoc` double NOT NULL,
  `max_good_eco2` double NOT NULL,
  `min_mid_eco2` double NOT NULL,
  `max_mid_eco2` double NOT NULL,
  `min_bad_eco2` double NOT NULL,
  `max_good_aqi` double NOT NULL,
  `min_mid_aqi` double NOT NULL,
  `max_mid_aqi` double NOT NULL,
  `min_bad_aqi` double NOT NULL,
  `room_id` bigint NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `room_id` (`room_id`),
  CONSTRAINT `eye_app_roomqualities_room_id_de439cdc_fk_eye_app_room_id` FOREIGN KEY (`room_id`) REFERENCES `eye_app_room` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_polish_ci;

-- Zrzucanie danych dla tabeli eye_db.eye_app_roomqualities: ~1 rows (około)
INSERT INTO `eye_app_roomqualities` (`id`, `max_bad_temp_otocz`, `min_mid_temp_otocz`, `max_mid_temp_otocz`, `min_good_temp_otocz`, `max_good_temp_otocz`, `min_mid2_temp_otocz`, `max_mid2_temp_otocz`, `min_bad2_temp_otocz`, `max_bad_temp_zbliz`, `min_mid_temp_zbliz`, `max_mid_temp_zbliz`, `min_good_temp_zbliz`, `max_good_temp_zbliz`, `min_mid2_temp_zbliz`, `max_mid2_temp_zbliz`, `min_bad2_temp_zbliz`, `max_bad_wiglotnosc`, `min_mid_wiglotnosc`, `max_mid_wiglotnosc`, `min_good_wiglotnosc`, `max_good_wiglotnosc`, `min_mid2_wiglotnosc`, `max_mid2_wiglotnosc`, `min_bad2_wiglotnosc`, `max_good_co2`, `min_mid_co2`, `max_mid_co2`, `min_bad_co2`, `max_good_tvoc`, `min_mid_tvoc`, `max_mid_tvoc`, `min_bad_tvoc`, `max_good_eco2`, `min_mid_eco2`, `max_mid_eco2`, `min_bad_eco2`, `max_good_aqi`, `min_mid_aqi`, `max_mid_aqi`, `min_bad_aqi`, `room_id`) VALUES
	(1, 15, 16, 18, 19, 22, 23, 25, 26, 15, 16, 18, 19, 22, 23, 25, 26, 20, 21, 39, 40, 60, 61, 80, 81, 600, 601, 1000, 1001, 50, 51, 750, 751, 600, 601, 1000, 1001, 1, 2, 3, 4, 1);

-- Zrzut struktury tabela eye_db.eye_app_sen22396
CREATE TABLE IF NOT EXISTS `eye_app_sen22396` (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `co2` double NOT NULL,
  `humidity` double NOT NULL,
  `temperature` double NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=76 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_polish_ci;

-- Zrzucanie danych dla tabeli eye_db.eye_app_sen22396: ~24 rows (około)
INSERT INTO `eye_app_sen22396` (`id`, `co2`, `humidity`, `temperature`) VALUES
	(1, 610, 63.34, 21.18),
	(2, 604, 63.4, 21.16),
	(3, 604, 63.29, 21.12),
	(4, 608, 63.35, 21.14),
	(5, 632, 63.29, 21.25),
	(6, 580, 62.71, 21.26),
	(7, 632, 63.29, 21.25),
	(8, 578, 62.62, 21.23),
	(9, 581, 62.66, 21.19),
	(10, 584, 62.67, 21.22),
	(11, 574, 62.6, 21.25),
	(12, 585, 63.05, 21.13),
	(13, 584, 63.07, 21.16),
	(14, 589, 63.02, 21.23),
	(15, 591, 62.82, 21.28),
	(16, 591, 62.84, 21.28),
	(17, 582, 62.91, 21.29),
	(18, 578, 62.84, 21.31),
	(19, 575, 62.76, 21.26),
	(20, 569, 62.64, 21.19),
	(21, 564, 62.48, 21.22),
	(22, 565, 62.44, 21.18),
	(23, 557, 62.49, 21.16),
	(24, 568, 62.39, 21.24),
	(25, 536, 62.1, 21.34),
	(26, 536, 62.1, 21.34),
	(27, 541, 62.09, 21.35),
	(28, 538, 61.93, 21.35),
	(29, 538, 62.08, 21.33),
	(30, 535, 61.86, 21.33),
	(31, 528, 61.81, 21.32),
	(32, 532, 61.94, 21.31),
	(33, 532, 61.87, 21.31),
	(34, 528, 61.67, 21.32),
	(35, 529, 61.69, 21.31),
	(36, 529, 61.69, 21.31),
	(37, 520, 61.58, 21.29),
	(38, 529, 61.69, 21.31),
	(39, 523, 61.48, 21.28),
	(40, 525, 61.64, 21.27),
	(41, 529, 61.69, 21.31),
	(42, 522, 61.69, 21.29),
	(43, 517, 61.72, 21.31),
	(44, 529, 61.69, 21.31),
	(45, 518, 61.55, 21.3),
	(46, 529, 61.69, 21.31),
	(47, 514, 61.56, 21.32),
	(48, 516, 61.34, 21.32),
	(49, 516, 61.27, 21.3),
	(50, 517, 61.4, 21.31),
	(51, 513, 61.34, 21.29),
	(52, 529, 61.69, 21.31),
	(53, 509, 61.23, 21.29),
	(54, 504, 61.25, 21.33),
	(55, 512, 61.24, 21.32),
	(56, 511, 61.09, 21.35),
	(57, 520, 61.05, 21.37),
	(58, 522, 60.98, 21.35),
	(59, 529, 61.69, 21.31),
	(60, 523, 60.98, 21.35),
	(61, 522, 60.93, 21.31),
	(62, 529, 61.69, 21.31),
	(63, 516, 60.68, 21.31),
	(64, 511, 60.69, 21.33),
	(65, 518, 60.62, 21.28),
	(66, 515, 60.51, 21.27),
	(67, 510, 60.42, 21.29),
	(68, 505, 60.36, 21.35),
	(69, 500, 60.06, 21.38),
	(70, 502, 60.08, 21.36),
	(71, 508, 60.08, 21.37),
	(72, 511, 59.96, 21.49),
	(73, 513, 59.71, 21.53),
	(74, 509, 59.76, 21.5),
	(75, 514, 59.7, 21.42);

-- Zrzut struktury tabela eye_db.eye_app_sensordevice
CREATE TABLE IF NOT EXISTS `eye_app_sensordevice` (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `device_uuid` varchar(64) COLLATE utf8mb4_polish_ci NOT NULL,
  `ip` varchar(50) COLLATE utf8mb4_polish_ci NOT NULL,
  `key` varchar(64) COLLATE utf8mb4_polish_ci NOT NULL,
  `last_seen` datetime(6) NOT NULL,
  `description` varchar(255) COLLATE utf8mb4_polish_ci DEFAULT NULL,
  `room_id` bigint DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `device_uuid` (`device_uuid`),
  UNIQUE KEY `ip` (`ip`),
  KEY `eye_app_sensordevice_room_id_82dfe896_fk_eye_app_room_id` (`room_id`),
  CONSTRAINT `eye_app_sensordevice_room_id_82dfe896_fk_eye_app_room_id` FOREIGN KEY (`room_id`) REFERENCES `eye_app_room` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_polish_ci;

-- Zrzucanie danych dla tabeli eye_db.eye_app_sensordevice: ~1 rows (około)
INSERT INTO `eye_app_sensordevice` (`id`, `device_uuid`, `ip`, `key`, `last_seen`, `description`, `room_id`) VALUES
	(1, '4a7c2f91b3e8d6cfc9a1be4fd3c2aa8e17f6a3b947ebd8c65b3f9d2484c7e12a', '10.0.13.214', 'abcdabcdabcdabcdabcdabcdabcdabcdabcdabcdabcdabcdabcdabcdabcdabcd', '2025-05-31 11:52:41.160000', NULL, NULL);

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
