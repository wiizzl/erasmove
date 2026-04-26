/* =========================================================================
   PROCEDURE : PSS_TRANSPORT
   DESCRIPTION : SELECTIONNE TOUS LES TRANSPORTS OU UN TRANSPORT SPECIFIQUE
   ========================================================================= */
CREATE PROCEDURE PSS_TRANSPORT
    @TRA_ID INT = NULL
AS
BEGIN
    IF @TRA_ID IS NULL
    BEGIN
        SELECT T.TRA_ID, T.TRA_COMPAGNIE, T.TYP_ID, TYP.TYP_LIBELLE
        FROM TRANSPORT T
        INNER JOIN TYPE_TRANSPORT TYP ON T.TYP_ID = TYP.TYP_ID
        ORDER BY T.TRA_COMPAGNIE;
    END
    ELSE
    BEGIN
        SELECT T.TRA_ID, T.TRA_COMPAGNIE, T.TYP_ID, TYP.TYP_LIBELLE
        FROM TRANSPORT T
        INNER JOIN TYPE_TRANSPORT TYP ON T.TYP_ID = TYP.TYP_ID
        WHERE T.TRA_ID = @TRA_ID;
    END
END;

/* =========================================================================
   PROCEDURE : PSI_TRANSPORT
   DESCRIPTION : INSERE UN NOUVEAU TRANSPORT DANS LA BASE DE DONNEES
   ========================================================================= */
CREATE PROCEDURE PSI_TRANSPORT
    @TRA_COMPAGNIE VARCHAR(100),
    @TYP_ID INT,
    @NEW_ID INT OUTPUT
AS
BEGIN
    INSERT INTO TRANSPORT (TRA_COMPAGNIE, TYP_ID)
    VALUES (@TRA_COMPAGNIE, @TYP_ID);

    SET @NEW_ID = SCOPE_IDENTITY();
END;

/* =========================================================================
   PROCEDURE : PSU_TRANSPORT
   DESCRIPTION : MET A JOUR LES INFORMATIONS D'UN TRANSPORT EXISTANT
   ========================================================================= */
CREATE PROCEDURE PSU_TRANSPORT
    @TRA_ID INT,
    @TRA_COMPAGNIE VARCHAR(100),
    @TYP_ID INT
AS
BEGIN
    UPDATE TRANSPORT
    SET TRA_COMPAGNIE = @TRA_COMPAGNIE,
        TYP_ID = @TYP_ID
    WHERE TRA_ID = @TRA_ID;
END;

/* =========================================================================
   PROCEDURE : PSD_TRANSPORT
   DESCRIPTION : SUPPRIME UN TRANSPORT DE LA BASE DE DONNEES
   ========================================================================= */
CREATE PROCEDURE PSD_TRANSPORT
    @TRA_ID INT
AS
BEGIN
    DELETE FROM TRANSPORT
    WHERE TRA_ID = @TRA_ID;
END;

/* =========================================================================
   PROCEDURE : PSS_TYPE_TRANSPORT
   DESCRIPTION : SELECTIONNE TOUS LES TYPES DE TRANSPORT DISPONIBLES
   ========================================================================= */
CREATE PROCEDURE PSS_TYPE_TRANSPORT
AS
BEGIN
    SELECT TYP_ID, TYP_LIBELLE
    FROM TYPE_TRANSPORT
    ORDER BY TYP_LIBELLE;
END;