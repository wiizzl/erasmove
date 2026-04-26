/* =========================================================================
   PROCEDURE : PSS_LIEU
   DESCRIPTION : SELECTIONNE TOUS LES LIEUX OU UN LIEU SPECIFIQUE SI L'ID EST RENSEIGNE
   ========================================================================= */
CREATE PROCEDURE PSS_LIEU
    @LIE_ID INT = NULL
AS
BEGIN
    IF @LIE_ID IS NULL
    BEGIN
        SELECT LIE_ID, LIE_NOM, LIE_VILLE, LIE_PAYS, LIE_LATITUDE, LIE_LONGITUDE
        FROM LIEU
        ORDER BY LIE_NOM;
    END
    ELSE
    BEGIN
        SELECT LIE_ID, LIE_NOM, LIE_VILLE, LIE_PAYS, LIE_LATITUDE, LIE_LONGITUDE
        FROM LIEU
        WHERE LIE_ID = @LIE_ID;
    END
END;

/* =========================================================================
   PROCEDURE : PSI_LIEU
   DESCRIPTION : INSERE UN NOUVEAU LIEU DANS LA BASE DE DONNEES
   ========================================================================= */
CREATE PROCEDURE PSI_LIEU
    @LIE_NOM VARCHAR(100),
    @LIE_VILLE VARCHAR(100),
    @LIE_PAYS VARCHAR(100),
    @LIE_LATITUDE FLOAT = NULL,
    @LIE_LONGITUDE FLOAT = NULL,
    @NEW_ID INT OUTPUT
AS
BEGIN
    INSERT INTO LIEU (LIE_NOM, LIE_VILLE, LIE_PAYS, LIE_LATITUDE, LIE_LONGITUDE)
    VALUES (@LIE_NOM, @LIE_VILLE, @LIE_PAYS, @LIE_LATITUDE, @LIE_LONGITUDE);

    SET @NEW_ID = SCOPE_IDENTITY();
END;

/* =========================================================================
   PROCEDURE : PSU_LIEU
   DESCRIPTION : MET A JOUR LES INFORMATIONS D'UN LIEU EXISTANT
   ========================================================================= */
CREATE PROCEDURE PSU_LIEU
    @LIE_ID INT,
    @LIE_NOM VARCHAR(100),
    @LIE_VILLE VARCHAR(100),
    @LIE_PAYS VARCHAR(100),
    @LIE_LATITUDE FLOAT = NULL,
    @LIE_LONGITUDE FLOAT = NULL
AS
BEGIN
    UPDATE LIEU
    SET LIE_NOM = @LIE_NOM,
        LIE_VILLE = @LIE_VILLE,
        LIE_PAYS = @LIE_PAYS,
        LIE_LATITUDE = @LIE_LATITUDE,
        LIE_LONGITUDE = @LIE_LONGITUDE
    WHERE LIE_ID = @LIE_ID;
END;

/* =========================================================================
   PROCEDURE : PSD_LIEU
   DESCRIPTION : SUPPRIME UN LIEU DE LA BASE DE DONNEES
   ========================================================================= */
CREATE PROCEDURE PSD_LIEU
    @LIE_ID INT
AS
BEGIN
    DELETE FROM LIEU
    WHERE LIE_ID = @LIE_ID;
END;