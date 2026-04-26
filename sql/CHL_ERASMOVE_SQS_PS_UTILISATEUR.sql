/* =========================================================================
   PROCEDURE : PSS_UTILISATEUR
   DESCRIPTION : SELECTIONNE TOUS LES UTILISATEURS OU UN UTILISATEUR SPECIFIQUE
   ========================================================================= */
CREATE PROCEDURE PSS_UTILISATEUR
    @UTI_ID INT = NULL
AS
BEGIN
    IF @UTI_ID IS NULL
    BEGIN
        SELECT U.UTI_ID, U.UTI_NOM, U.UTI_PRENOM, U.UTI_LOGIN, U.ROL_ID, R.ROL_LIBELLE
        FROM UTILISATEUR U
        INNER JOIN ROLE R ON U.ROL_ID = R.ROL_ID
        ORDER BY U.UTI_NOM;
    END
    ELSE
    BEGIN
        SELECT U.UTI_ID, U.UTI_NOM, U.UTI_PRENOM, U.UTI_LOGIN, U.ROL_ID, R.ROL_LIBELLE
        FROM UTILISATEUR U
        INNER JOIN ROLE R ON U.ROL_ID = R.ROL_ID
        WHERE U.UTI_ID = @UTI_ID;
    END
END;

/* =========================================================================
   PROCEDURE : PSS_UTILISATEUR_LOGIN
   DESCRIPTION : VERIFIE LES IDENTIFIANTS DE CONNEXION D'UN UTILISATEUR
   ========================================================================= */
CREATE PROCEDURE PSS_UTILISATEUR_LOGIN
    @UTI_LOGIN VARCHAR(50),
    @UTI_MOTDEPASSE VARCHAR(255)
AS
BEGIN
    SELECT U.UTI_ID, U.UTI_NOM, U.UTI_PRENOM, U.UTI_LOGIN, U.ROL_ID, R.ROL_LIBELLE
    FROM UTILISATEUR U
    INNER JOIN ROLE R ON U.ROL_ID = R.ROL_ID
    WHERE U.UTI_LOGIN = @UTI_LOGIN 
      AND U.UTI_MOTDEPASSE = @UTI_MOTDEPASSE;
END;

/* =========================================================================
   PROCEDURE : PSI_UTILISATEUR
   DESCRIPTION : INSERE UN NOUVEL UTILISATEUR DANS LA BASE DE DONNEES
   ========================================================================= */
CREATE PROCEDURE PSI_UTILISATEUR
    @UTI_NOM VARCHAR(50),
    @UTI_PRENOM VARCHAR(50),
    @UTI_LOGIN VARCHAR(50),
    @UTI_MOTDEPASSE VARCHAR(255),
    @ROL_ID INT,
    @NEW_ID INT OUTPUT
AS
BEGIN
    INSERT INTO UTILISATEUR (UTI_NOM, UTI_PRENOM, UTI_LOGIN, UTI_MOTDEPASSE, ROL_ID)
    VALUES (@UTI_NOM, @UTI_PRENOM, @UTI_LOGIN, @UTI_MOTDEPASSE, @ROL_ID);

    SET @NEW_ID = SCOPE_IDENTITY();
END;

/* =========================================================================
   PROCEDURE : PSU_UTILISATEUR
   DESCRIPTION : MET A JOUR LES INFORMATIONS D'UN UTILISATEUR EXISTANT
   ========================================================================= */
CREATE PROCEDURE PSU_UTILISATEUR
    @UTI_ID INT,
    @UTI_NOM VARCHAR(50),
    @UTI_PRENOM VARCHAR(50),
    @UTI_LOGIN VARCHAR(50),
    @UTI_MOTDEPASSE VARCHAR(255),
    @ROL_ID INT
AS
BEGIN
    UPDATE UTILISATEUR
    SET UTI_NOM = @UTI_NOM,
        UTI_PRENOM = @UTI_PRENOM,
        UTI_LOGIN = @UTI_LOGIN,
        UTI_MOTDEPASSE = @UTI_MOTDEPASSE,
        ROL_ID = @ROL_ID
    WHERE UTI_ID = @UTI_ID;
END;

/* =========================================================================
   PROCEDURE : PSD_UTILISATEUR
   DESCRIPTION : SUPPRIME UN UTILISATEUR DE LA BASE DE DONNEES
   ========================================================================= */
CREATE PROCEDURE PSD_UTILISATEUR
    @UTI_ID INT
AS
BEGIN
    DELETE FROM UTILISATEUR
    WHERE UTI_ID = @UTI_ID;
END;

/* =========================================================================
   PROCEDURE : PSS_ROLE
   DESCRIPTION : LISTE LES ROLES
   ========================================================================= */
CREATE PROCEDURE PSS_ROLE
AS
BEGIN
    SELECT ROL_ID, ROL_LIBELLE
    FROM ROLE
    ORDER BY ROL_LIBELLE;
END;