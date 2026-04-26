/* =========================================================================
   PROCEDURE : PSS_TRAJET
   DESCRIPTION : SELECTIONNE TOUS LES SEGMENTS DU CATALOGUE OU UN SPECIFIQUE
   ========================================================================= */
CREATE PROCEDURE PSS_TRAJET
    @TRJ_ID INT = NULL
AS
BEGIN
    SELECT TRJ.TRJ_ID, TRJ.LIE_ID_DEPART,
           LD.LIE_VILLE AS VILLE_DEPART, LD.LIE_PAYS AS PAYS_DEPART, 
           LD.LIE_LATITUDE AS LAT_DEPART, LD.LIE_LONGITUDE AS LON_DEPART,
           TRJ.LIE_ID_ARRIVEE, LA.LIE_VILLE AS VILLE_ARRIVEE, LA.LIE_PAYS AS PAYS_ARRIVEE,
           LA.LIE_LATITUDE AS LAT_ARRIVEE, LA.LIE_LONGITUDE AS LON_ARRIVEE,
           TRJ.TRA_ID, T.TRA_COMPAGNIE, TYP.TYP_LIBELLE
    FROM TRAJET TRJ
    INNER JOIN LIEU LD ON TRJ.LIE_ID_DEPART = LD.LIE_ID
    INNER JOIN LIEU LA ON TRJ.LIE_ID_ARRIVEE = LA.LIE_ID
    INNER JOIN TRANSPORT T ON TRJ.TRA_ID = T.TRA_ID
    INNER JOIN TYPE_TRANSPORT TYP ON T.TYP_ID = TYP.TYP_ID
    WHERE (@TRJ_ID IS NULL OR TRJ.TRJ_ID = @TRJ_ID);
END;

/* =========================================================================
   PROCEDURE : PSI_TRAJET
   DESCRIPTION : INSERE UN NOUVEAU SEGMENT DANS LE CATALOGUE
   ========================================================================= */
CREATE PROCEDURE PSI_TRAJET
    @LIE_ID_DEPART INT,
    @LIE_ID_ARRIVEE INT,
    @TRA_ID INT,
    @NEW_ID INT OUTPUT
AS
BEGIN
    INSERT INTO TRAJET (LIE_ID_DEPART, LIE_ID_ARRIVEE, TRA_ID)
    VALUES (@LIE_ID_DEPART, @LIE_ID_ARRIVEE, @TRA_ID);

    SET @NEW_ID = SCOPE_IDENTITY();
END;

/* =========================================================================
   PROCEDURE : PSU_TRAJET
   DESCRIPTION : MET A JOUR UN SEGMENT DU CATALOGUE
   ========================================================================= */
CREATE PROCEDURE PSU_TRAJET
    @TRJ_ID INT,
    @LIE_ID_DEPART INT,
    @LIE_ID_ARRIVEE INT,
    @TRA_ID INT
AS
BEGIN
    UPDATE TRAJET
    SET LIE_ID_DEPART = @LIE_ID_DEPART,
        LIE_ID_ARRIVEE = @LIE_ID_ARRIVEE,
        TRA_ID = @TRA_ID
    WHERE TRJ_ID = @TRJ_ID;
END;

/* =========================================================================
   PROCEDURE : PSD_TRAJET
   DESCRIPTION : SUPPRIME UN SEGMENT DU CATALOGUE
   ========================================================================= */
CREATE PROCEDURE PSD_TRAJET
    @TRJ_ID INT
AS
BEGIN
    DELETE FROM TRAJET WHERE TRJ_ID = @TRJ_ID;
END;